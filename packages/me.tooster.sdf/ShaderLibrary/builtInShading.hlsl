#pragma once

/*
 * Passes of the built-in render pipeline for raymarched SDF scenes: forward base, forward add and shadow caster.
 *
 * Every fragment marches its own ray, which it builds from the current view and projection matrices. The same code
 * therefore renders for the camera, for the camera depth texture and for shadow maps, where the light takes the place of
 * the camera. Lighting follows the Standard shader: Unity's PBS lighting from UnityPBSLighting.cginc, with shadows and
 * light attenuation computed at the raymarched hit instead of at the domain mesh.
 */

#include "UnityCG.cginc"
#include "UnityPBSLighting.cginc"
#include "AutoLight.cginc"

#include "camera.hlsl"
#include "noise.hlsl"
#include_with_pragmas "raymarching.hlsl"
#include "types.hlsl"

#pragma shader_feature_local _ZWRITE_ON _ZWRITE_OFF

#ifndef EXPLICIT_RAYMARCHING_PARAMETERS
int   _AO_STEPS = 6;
float _AO_MAX_DISTANCE = 1;
float _AO_FALLOFF = 2.5;
#endif

#pragma region forward declarations
/// Returns the material of the surface that \p sdf hit
Material SdfMaterial(SdfResult sdf);
#pragma endregion

#pragma region raymarching

/// Output of the vertex shaders of every pass
struct SdfVertexOutput {
    float4 pos : SV_POSITION;
    /// A copy of the clip-space position, which the fragment receives with perspective-correct interpolation and turns
    /// into its ray. SV_POSITION can't be used for this: the fragment receives it in pixels.
    float4 clipPos : TEXCOORD0;
};

SdfVertexOutput sdfVertex(appdata_base v) {
    SdfVertexOutput o;
    o.pos = UnityObjectToClipPos(v.vertex);
    o.clipPos = o.pos;
    return o;
}

/// Marches the ray of a fragment and discards the fragment if the ray hits nothing.
SdfResult marchFragment(float4 clipPos, out Ray3D ray) {
    ray = viewRayFromClipPos(clipPos);
    ray.ro += ray.rd * _RAY_ORIGIN_BIAS;
    ray.maxDistance = _MAX_DISTANCE;
    SdfResult sdf = raymarch(ray, _MAX_STEPS, _EPSILON_RAY);
    if (!isHit(sdf))
        discard;
    return sdf;
}

/// Returns the value for SV_Depth of a world-space position
float clipSpaceDepth(float3 worldPos) {
    float4 clipPos = UnityWorldToClipPos(worldPos);
    return clipPos.z / clipPos.w;
}

#pragma endregion

#pragma region lighting

/// Returns the material of the objects at the hit, blended where they meet, with occlusion from the SDF. The scene is
/// evaluated once more at the hit: marching only reads distances and IDs, so the compiler removes the material code
/// from the marching loop.
Material SdfMaterial(SdfResult sdf) {
    Material m = sdfScene(sdf.p).material;
    m.occlusion *= classicAmbientOcclusion(sdf.p, sdf.normal, _AO_MAX_DISTANCE, _AO_FALLOFF, _AO_STEPS);
    return m;
}

SurfaceOutputStandard surfaceOutput(SdfResult sdf, Material mat) {
    SurfaceOutputStandard o = (SurfaceOutputStandard)0;
    o.Albedo = mat.albedo.rgb;
    o.Metallic = mat.metallic;
    o.Smoothness = mat.smoothness;
    o.Emission = mat.emission;
    o.Occlusion = mat.occlusion;
    o.Alpha = 1;
    o.Normal = sdf.normal;
    return o;
}

/// The inputs that the AutoLight.cginc macros read, filled in for a raymarched hit. The values that the vertex shader
/// would interpolate belong to the domain mesh, so shadows would be sampled at the wrong place.
struct SdfLightInput {
    float4            pos;
    unityShadowCoord4 _ShadowCoord;
    unityShadowCoord4 _LightCoord;
};

/// Returns the attenuation and the realtime shadow of the light of the current forward pass at \p worldPos
half lightAttenuation(float3 worldPos) {
    SdfLightInput input = (SdfLightInput)0;
    input.pos = UnityWorldToClipPos(worldPos);
    #if defined(POINT) || defined(SPOT) || defined(POINT_COOKIE) || defined(DIRECTIONAL_COOKIE)
    input._LightCoord = mul(unity_WorldToLight, unityShadowCoord4(worldPos, 1));
    #endif
    #if defined(SHADOWS_SCREEN) && !defined(UNITY_NO_SCREENSPACE_SHADOWS) && !defined(LIGHTMAP_ON) && defined(SHADOWS_SHADOWMASK)
    // with a shadow mask, the macros read the screen position from zw and the lightmap coordinates from xy
    input._ShadowCoord = unityShadowCoord4(0, 0, ComputeScreenPos(input.pos).xy);
    #elif defined(SHADOWS_SCREEN) && !defined(UNITY_NO_SCREENSPACE_SHADOWS)
    // the screen-space shadow map is built from the camera depth texture, which the shadow caster pass fills with the
    // raymarched depth, so the hit reads its own pixel
    input._ShadowCoord = ComputeScreenPos(input.pos);
    #elif defined(SHADOWS_SCREEN) || (defined(SHADOWS_DEPTH) && defined(SPOT))
    input._ShadowCoord = mul(unity_WorldToShadow[0], unityShadowCoord4(worldPos, 1));
    #elif defined(SHADOWS_CUBE)
    input._ShadowCoord = unityShadowCoord4(worldPos - _LightPositionRange.xyz, 1);
    #endif

    UNITY_LIGHT_ATTENUATION(atten, input, worldPos);
    return atten;
}

/// Standard lighting of the main directional light, ambient light and reflection probes, for the forward base pass
half4 shadeForwardBase(SdfResult sdf, Material mat) {
    SurfaceOutputStandard o = surfaceOutput(sdf, mat);
    float3                worldPos = sdf.p;
    half3                 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));

    UnityGI gi = (UnityGI)0;
    gi.light.color = _LightColor0.rgb;
    gi.light.dir = normalize(UnityWorldSpaceLightDir(worldPos));

    UnityGIInput giInput = (UnityGIInput)0;
    giInput.light = gi.light;
    giInput.worldPos = worldPos;
    giInput.worldViewDir = worldViewDir;
    giInput.atten = lightAttenuation(worldPos);
    giInput.lightmapUV = 0;
    #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    // the per-pixel part (L0 and L1) is added by the GI function, which expects the per-vertex part (L2) here
    giInput.ambient = SHEvalLinearL2(half4(o.Normal, 1));
    #endif
    giInput.probeHDR[0] = unity_SpecCube0_HDR;
    giInput.probeHDR[1] = unity_SpecCube1_HDR;
    #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION) || defined(UNITY_ENABLE_REFLECTION_BUFFERS)
    giInput.boxMin[0] = unity_SpecCube0_BoxMin;
    #endif
    #ifdef UNITY_SPECCUBE_BOX_PROJECTION
    giInput.boxMax[0] = unity_SpecCube0_BoxMax;
    giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
    giInput.boxMax[1] = unity_SpecCube1_BoxMax;
    giInput.boxMin[1] = unity_SpecCube1_BoxMin;
    giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
    #endif

    LightingStandard_GI(o, giInput, gi);
    half4 color = LightingStandard(o, worldViewDir, gi);
    color.rgb += o.Emission;
    color.a = 1;
    return color;
}

/// Standard lighting of one additional light, for the forward add pass
half4 shadeForwardAdd(SdfResult sdf, Material mat) {
    SurfaceOutputStandard o = surfaceOutput(sdf, mat);
    float3                worldPos = sdf.p;

    UnityGI gi = (UnityGI)0;
    gi.light.color = _LightColor0.rgb * lightAttenuation(worldPos);
    gi.light.dir = normalize(UnityWorldSpaceLightDir(worldPos));

    half4 color = LightingStandard(o, normalize(UnityWorldSpaceViewDir(worldPos)), gi);
    color.a = 0;
    return color;
}

/// Applies the scene's fog at \p worldPos. The forward add pass fades to black, so that the fog color is added once.
half4 applyFog(half4 color, float3 worldPos, half4 fogColor) {
    #if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
    UNITY_CALC_FOG_FACTOR(UnityWorldToClipPos(worldPos).z);
    UNITY_FOG_LERP_COLOR(color, fogColor, unityFogFactor);
    #endif
    return color;
}

#pragma endregion

#pragma region forward passes

struct SdfFragmentOutput {
    half4 color : SV_Target;
    #ifdef _ZWRITE_ON
    float depth : SV_Depth; ///< the depth of the hit, so that other geometry and the depth texture agree with the SDF
    #endif
};

/// Vertex shader of the forward base pass. The name is kept for scene presets that select it with a pragma.
SdfVertexOutput vertexShader(appdata_base v) { return sdfVertex(v); }

/// Fragment shader of the forward base pass. The name is kept for scene presets that select it with a pragma.
SdfFragmentOutput fragmentShader(SdfVertexOutput i) {
    Ray3D     ray;
    SdfResult sdf = marchFragment(i.clipPos, ray);
    sdf.normal = calculateSdfNormal(sdf.p, _EPSILON_NORMAL);

    Material mat = SdfMaterial(sdf);
    half4    color = shadeForwardBase(sdf, mat);
    color = debugOverlay(color, mat, sdf, ray);

    SdfFragmentOutput o;
    o.color = applyFog(color, sdf.p, unity_FogColor);
    #ifdef _ZWRITE_ON
    o.depth = clipSpaceDepth(sdf.p);
    #endif
    return o;
}

SdfVertexOutput forwardAddVertex(appdata_base v) { return sdfVertex(v); }

SdfFragmentOutput forwardAddFragment(SdfVertexOutput i) {
    Ray3D     ray;
    SdfResult sdf = marchFragment(i.clipPos, ray);
    sdf.normal = calculateSdfNormal(sdf.p, _EPSILON_NORMAL);

    SdfFragmentOutput o;
    #ifdef _DRAWMODE_MATERIAL
    o.color = applyFog(shadeForwardAdd(sdf, SdfMaterial(sdf)), sdf.p, 0);
    #else
    o.color = 0; // the debug draw modes show the forward base pass only
    #endif
    #ifdef _ZWRITE_ON
    o.depth = clipSpaceDepth(sdf.p);
    #endif
    return o;
}

#pragma endregion

#pragma region shadow caster pass

/// Returns the clip-space position of a shadow caster at \p worldPos, moved along its normal by the light's normal bias.
/// A world-space version of UnityClipSpaceShadowCasterPos from UnityCG.cginc.
float4 shadowCasterClipPos(float3 worldPos, float3 worldNormal) {
    if (unity_LightShadowBias.z != 0.0) {
        float3 toLight = normalize(UnityWorldSpaceLightDir(worldPos));
        float  shadowCos = dot(worldNormal, toLight);
        float  shadowSine = sqrt(1 - shadowCos * shadowCos);
        worldPos -= worldNormal * unity_LightShadowBias.z * shadowSine;
    }
    return UnityWorldToClipPos(worldPos);
}

SdfVertexOutput shadowCasterVertex(appdata_base v) { return sdfVertex(v); }

/// Writes the depth of the hit into a shadow map or the camera depth texture, which this pass also renders. Point light
/// shadows on platforms without depth cube maps store the distance to the light as a color.
float4 shadowCasterFragment(SdfVertexOutput i, out float depth : SV_Depth) : SV_Target {
    Ray3D     ray;
    SdfResult sdf = marchFragment(i.clipPos, ray);
    #if defined(SHADOWS_CUBE) && !defined(SHADOWS_CUBE_IN_DEPTH_TEX)
    depth = clipSpaceDepth(sdf.p);
    return UnityEncodeCubeShadowDepth((length(sdf.p - _LightPositionRange.xyz) + unity_LightShadowBias.x) * _LightPositionRange.w);
    #else
    float3 normal = 0;
    UNITY_BRANCH
    if (unity_LightShadowBias.z != 0.0)
        normal = calculateSdfNormal(sdf.p, _EPSILON_NORMAL);
    float4 clipPos = UnityApplyLinearShadowBias(shadowCasterClipPos(sdf.p, normal));
    depth = clipPos.z / clipPos.w;
    return 0;
    #endif
}

#pragma endregion
