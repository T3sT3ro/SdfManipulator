#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "UnityPBSLighting.cginc"
#include "AutoLight.cginc"

#include "types.hlsl"
#include "camera.hlsl"
#include "debug.hlsl"
#include "noise.hlsl"
#include "colors.hlsl"
#include "raymarching.hlsl"

#pragma shader_feature_local _ZWRITE_ON _ZWRITE_OFF

#ifndef EXPLICIT_RAYMARCHING_PARAMETERS
int   _AO_STEPS = 6;
float _AO_MAX_DISTANCE = 1;
float _AO_FALLOFF = 2.5;
#endif

#pragma region forward declarations
/// A function for shading the SdfResult
fixed4 sdfShade(SdfResult sdf, Ray3D ray);
/// A function for determining a material at a given point
Material SdfMaterial(SdfResult sdf);

v2f vertexShader(in appdata_base v_in);
f2p fragmentShader(in v2f frag_in, bool facing : SV_IsFrontFace);
#pragma endregion

#pragma region vertex and fragment

/// standard vertex shader with additional parameters for raymarching
v2f vertexShader(in appdata_base v_in) {
    v2f o = (v2f)0;
    o.vertex = UnityObjectToClipPos(v_in.vertex); // clip space, from (-w,-w,0) to (w, w, w)
    o.screenPos = ComputeScreenPos(o.vertex); // from 0,0 to 1,1
    // o.uv = v.texcoord; // TRANSFORM_TEX(v.texcoord, _BoxmapTex);
    o.hitpos = v_in.vertex;

    // COMPUTE_EYEDEPTH uses implicitly defined v.vertex.z... so to use `screenPos.z` instead of `o.vertex.z` name, we have to inline it ourselves:
    o.screenPos.z = -UnityObjectToViewPos(o.vertex).z;
    #ifdef V2F_RAYS
    o.rdWsUnnormalized = cameraVsRayFromClipPos(o.screenPos, o.roWs);
    #endif
    return o;
}

f2p fragmentShader(in v2f frag_in, bool facing : SV_IsFrontFace) {
    // float3 clipPos = frag_in.screenPos.xyz / frag_in.screenPos.w; // 0,0 to 1,1 on screen
    Ray3D ray = (Ray3D)0;

    #ifdef V2F_RAYS
    ray.rd = normalize(frag_in.rdWsUnnormalized);
    ray.ro = frag_in.roWs + ray.rd * _RAY_ORIGIN_BIAS;
    #else
    ray.rd = cameraRayFromClipPos(frag_in.screenPos, ray.ro);
    #endif

    ray.maxDistance = _MAX_DISTANCE; // FIXME: parametrize this
    SdfResult sdf = raymarch(ray, _MAX_STEPS, _EPSILON_RAY);

    clip(sdf.id.w); // discard rays without hit

    sdf.normal = calculateSdfNormal(sdf.p, _EPSILON_NORMAL);
    f2p frag_out = (f2p)0;
    frag_out.color = sdfShade(sdf, ray);
    // frag_out.color = facing ? YELLOW : RED;

    #ifdef _ZWRITE_ON
    float eyeDepth = -UnityWorldToViewPos(sdf.p).z;
    frag_out.depth = EncodeCorrectDepth(eyeDepth);
    #endif

    return frag_out;
}

#pragma endregion


#pragma region sdf functions

// TODO: only use forward declaration, generate SdfMaterial based on SDF scene
Material SdfMaterial(SdfResult sdf) {
    Material m = (Material)0;
    m.albedo = noise::randomColor(sdf.id.w);
    m.metallic = .6;
    m.emission = .1;
    m.occlusion = classicAmbientOcclusion(sdf.p, sdf.normal, _AO_MAX_DISTANCE, _AO_FALLOFF, _AO_STEPS);
    m.smoothness = .5;
    return m;
}

fixed3 lightDirection(float3 worldPosition) {
    #ifdef USING_DIRECTIONAL_LIGHT
    return _WorldSpaceLightPos0.xyz;
    #else
    return normalize(UnityWorldSpaceLightDir(worldPosition));
    #endif
}

fixed4 shadeAmbientLambert(SdfResult sdf, Material mat) {
    float  atten = 1.0;
    float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
    float3 diffuseReflection = atten * _LightColor0.xyz * max(0.0, dot(sdf.normal, lightDirection));
    float3 lightFinal = diffuseReflection + UNITY_LIGHTMODEL_AMBIENT.xyz;

    return float4(lightFinal * mat.albedo, 1.0);
}

/** \brief temporary function for shading sdf result
  * \remark TODO: return Material from combination of sdfs. Apply material lighting functions outside of sdfShade
  */
fixed4 sdfShade(SdfResult sdf, Ray3D ray) {
    // frag_out.color = sdf.id == NO_ID ? float4(1.0, 1.0, 0, 1.0) : float4(1.0, 0.1, 0.1, 1.0); // yellow or tomato
    // float dist = modulo(ray.marchedDistance, 0.1) / 0.1;
    // fixed3 distColor = dot(CameraWsForward(), ray.rd) * dist; // color by distance

    // SurfaceOutput s = (SurfaceOutput)0;
    // s.Albedo = colors::RED;
    // s.Alpha = 1.0;
    // s.Normal = sdf.normal;
    // s.Gloss = 0.5;
    // s.Specular = 0.9;
    //
    // UnityLight l = (UnityLight)0;
    // l.color = _LightColor0;
    // float3 toLight = _WorldSpaceLightPos0 - sdf.p;
    // l.dir = normalize(toLight);
    //
    // fixed4 color = UnityLambertLight(s, l);
    // color.rgb += ShadeSH9(half4(sdf.normal, 1));
    //
    // fixed4 gridColor = sdf::debug::grid(sdf.p);
    // color.rgb = lerp(color, gridColor.rgb, gridColor.a);
    //
    // color.rgb += s.Albedo * .1;
    // color.a = 1;

    Material mat = SdfMaterial(sdf);
    fixed4   color = shadeAmbientLambert(sdf, mat);
    color = debugOverlay(color, mat, sdf, ray);


    // // raymarch from point using hit point and normal to calculate shadows
    // Ray3D shadowRay = (Ray3D)0;
    // shadowRay.rd = normalize(toLight);
    // shadowRay.ro = sdf.p + shadowRay.rd + 2 * _EPSILON_RAY;
    // shadowRay.maxDistance = length(toLight);
    // SdfResult shadowSdf = raymarch(shadowRay, 128, 0.01);
    //
    // if (shadowSdf.id.w != NO_ID.w) // if something was hit, we are in full shadow
    //     color.rgb = 0;
    //
    // color = sdf::debug::visualizeNormal(l.dir);

    return color;
}

# pragma endregion
