#pragma once

#define V2F_RAYS

#include "UnityCG.cginc"
#include "util.hlsl"
#include "Packages/me.tooster.sdf/Editor/Resources/Includes/types.hlsl"
#include "Packages/me.tooster.sdf/Editor/Resources/Includes/camera.hlsl"
#include "Packages/me.tooster.sdf/Editor/Resources/Includes/debug.hlsl"
#include "Packages/me.tooster.sdf/Editor/Resources/Includes/noise.hlsl"

#pragma region Forward declarations

v2f vertexShader(in appdata_base v_in);
f2p fragmentShader(in v2f frag_in);
/// A scene SignedDistanceField function, defined in the included shader
SdfResult sdfScene(in float3 p);
fixed4    sdfShade(SdfResult sdf, Ray3D ray);
SdfResult raymarch(inout Ray3D ray, in int max_steps, in float epsilon_ray);
float3    calculateSdfNormal(in float3 p, in float epsilon_normal);
float     depthToMaxRayDepth(in Texture2D depthTexture, in float2 screenUV, in float3 rd, in float4x4 inv);
float     normalSdfOcclusion(in float3 p, in float3 normal, in int i, in float step_size);
float     classicAmbientOcclusion(float3 p, float3 n, float maxDist, float falloff, in int aoSteps);

#pragma endregion

#ifndef EXPLICIT_RAYMARCHING_PARAMETERS
static float _EPSILON_RAY = 0.0001;
static float _EPSILON_NORMAL = 0.0001;
static float _EPSILON_OCCLUSION = 0.01;
static float _MAX_DISTANCE = 10000;
static float _RAY_ORIGIN_BIAS;
static float _MAX_STEPS = 500;
UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
#endif

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

/// \TODO remove lighting definitions from this file, it's just temporary
#include "Lighting.cginc"
#include "colors.hlsl"
#include "UnityPBSLighting.cginc"
#pragma shader_feature_local _ZWRITE_ON _ZWRITE_OFF

f2p fragmentShader(in v2f frag_in, bool facing : SV_IsFrontFace) {
    // float3 clipPos = frag_in.screenPos.xyz / frag_in.screenPos.w; // 0,0 to 1,1 on screen
    Ray3D ray = (Ray3D)0;

    #ifdef V2F_RAYS
    ray.rd = normalize(frag_in.rdWsUnnormalized);
    ray.ro = frag_in.roWs;
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

# pragma region raymarching functions

/**
 * \brief reads the _CameraDepthTexture (rendered BEFORE this shader!) and returns the distance along rd to reach for that depth
 * \param screenUV a (0,0)->(1,1) screen-space coordinate of the pixel to read depth from
 * \param rd a ray direction from the camera
 * \param inv an inverse projection+world+model* matrix. *depends on the local frame 
 * \return returns a distance along the ray rd to reach for the depth at screenUV
 * \remark TODO: simplify to avoid inverse projection and both rd and uv
 */
float depthToMaxRayDepth(in sampler2D depthTexture, in float2 screenUV, in float3 rd, in float4x4 inv) {
    // read camera depth texture to correctly blend with scene geometry
    // beware, that _CameraDepthTexture IS NOT the depth buffer!
    // it is populated in the prepass and doesn't change in subsequent passes
    // https://forum.unity.com/threads/does-depth-buffer-update-between-passes.620575/
    float camDepth = CorrectDepth(tex2D(depthTexture, screenUV).rg);

    float4 forward = mul(inv, float4(0, 0, 1, 1)); // central ray end on far plane
    forward /= forward.w;
    forward = normalize(forward); // forward in object space
    return camDepth / dot(forward, rd);
}

// https://iquilezles.org/www/articles/normalsSDF/normalsSDF.htm
float3 sampleNormal(in float3 p, in float epsilon_normal) {
    // using tetrahedron technique
    // EPSILON -- can be adjusted using pixel footprint
    const float2 k = float2(1, -1);
    return normalize(
        k.xyy * sdfScene(p + k.xyy * epsilon_normal).distance +
        k.yyx * sdfScene(p + k.yyx * epsilon_normal).distance +
        k.yxy * sdfScene(p + k.yxy * epsilon_normal).distance +
        k.xxx * sdfScene(p + k.xxx * epsilon_normal).distance
    );
}

// returns sdf and ray point
SdfResult raymarch(inout Ray3D ray, in int max_steps, in float epsilon_ray) {
    SdfResult sdf = (SdfResult)0;
    sdf.distance = ray.maxDistance;
    sdf.id = int4(NO_ID);

    sdf.material = (Material)0;
    sdf.normal = 0;
    // ray can start inside SDF, this implementation makes it perform one step onto the surface
    // is it good or bad, well, depends on the use case 
    for (ray.steps = 0; ray.steps < max_steps; ray.steps++) {
        if (ray.marchedDistance >= ray.maxDistance) // there was a check before for ray.max distance OR _MAX_DISTANCE... but it can just be done in the outer scope
            return sdf;

        sdf.p = ray.ro + ray.marchedDistance * ray.rd;

        SdfResult sdfOnRay = sdfScene(sdf.p);
        if (sdfOnRay.distance < epsilon_ray) {
            sdf.distance = ray.marchedDistance;
            sdf.id = sdfOnRay.id;
            return sdf;
        }

        ray.marchedDistance += sdfOnRay.distance;
    }
    return sdf;
}

/**
 * \brief calculates normal as a gradient of the sdf using tetrahedron technique
 * \remarks see <a href="https://iquilezles.org/www/articles/normalsSDF/normalsSDF.htm">calculating normals by iq</a>
 */
float3 calculateSdfNormal(in float3 p, in float epsilon_normal) {
    // using tetrahedron technique
    // EPSILON -- can be adjusted using pixel footprint
    const float2 k = float2(1, -1);
    return normalize(
        k.xyy * sdfScene(p + k.xyy * epsilon_normal).distance +
        k.yyx * sdfScene(p + k.yyx * epsilon_normal).distance +
        k.yxy * sdfScene(p + k.yxy * epsilon_normal).distance +
        k.xxx * sdfScene(p + k.xxx * epsilon_normal).distance
    );
}

#pragma endregion

# pragma region shading

/** \brief temporary function for shading sdf result
  * \remark TODO: return Material from combination of sdfs. Apply material lighting functions outside of sdfShade
  */
fixed4 sdfShade(SdfResult sdf, Ray3D ray) {
    // frag_out.color = sdf.id == NO_ID ? float4(1.0, 1.0, 0, 1.0) : float4(1.0, 0.1, 0.1, 1.0); // yellow or tomato
    // float dist = modulo(ray.marchedDistance, 0.1) / 0.1;
    // fixed3 distColor = dot(CameraWsForward(), ray.rd) * dist; // color by distance

    SurfaceOutput s = (SurfaceOutput)0;
    s.Albedo = colors::RED;
    s.Alpha = 1.0;
    s.Normal = sdf.normal;
    s.Gloss = 0.5;
    s.Specular = 0.9;

    UnityLight l = (UnityLight)0;
    l.color = _LightColor0;
    float3 toLight = _WorldSpaceLightPos0 - sdf.p;
    l.dir = normalize(toLight);

    fixed4 color = UnityLambertLight(s, l);
    color.rgb += ShadeSH9(half4(sdf.normal, 1));

    fixed4 gridColor = sdf::debug::worldgrid(sdf.p);
    color.rgb = lerp(color, gridColor.rgb, gridColor.a);

    color.rgb += s.Albedo * .1;
    color.a = 1;

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

    // dampen the color by occlusion
    float occlusion = classicAmbientOcclusion(sdf.p, sdf.normal, 4, 2.5, 6);

    color.rgb = lerp(color.rgb, colors::BLACK, 1 - occlusion);

    return color;
}

/**
 * \brief Calculates simple occlusion
 * \param p point to calculate sdf occlusion for
 * \param normal normal at point p
 * \param i number of occlusion steps
 * \return occlusion in a range [0 (none) ... 1 (full)]
 * \remarks <a href="https://typhomnt.github.io/teaching/ray_tracing/raymarching_intro/">occlusion by typhomnt</a>
 */
float normalSdfOcclusion(in float3 p, in float3 normal, in int i, in float step_size) {
    float occlusion = 1.0;
    while (i > 0) { // steps used as a counter
        occlusion -= pow(i * i - sdfScene(p + normal * i * step_size).distance, 2) / i;
        i--;
    }
    return occlusion;
}

/**
 * Calculates ambient occlusion
 * @param p point to calculate sdf occlusion for
 * @param n normal at point p
 * @param maxDist max distance for occlusion
 * @param falloff how much to reduce the occlusion
 * @param aoSteps how many steps to apply, 6 is a godo tradeoff between quality and performance
 * @return an occlusion in a range [0 (none) ... 1 (full)]
 * @remarks <a href="https://www.shadertoy.com/view/4sdGWN">occlusion by XT95</a>
 */
float classicAmbientOcclusion(float3 p, float3 n, float maxDist, float falloff, in int aoSteps) {
    float ao = 0.0;
    for (int i = 0; i < aoSteps; i++) {
        float  l = noise::hash(float(i)) * maxDist;
        float3 rd = n * l;

        ao += (l - max(sdfScene(p + rd).distance, 0.0)) / maxDist * falloff;
    }

    return clamp(1.0 - ao / float(aoSteps), 0.0, 1.0);
}

#pragma endregion
