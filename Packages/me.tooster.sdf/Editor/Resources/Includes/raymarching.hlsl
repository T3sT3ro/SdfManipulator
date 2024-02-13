#pragma once
#include "UnityCG.cginc"
#include "util.hlsl"
#include "Packages/me.tooster.sdf/Editor/Resources/Includes/types.hlsl"
#include "Packages/me.tooster.sdf/Editor/Resources/Includes/camera.hlsl"
#include "Packages/me.tooster.sdf/Editor/Resources/Includes/debug.hlsl"

v2f vertexShader(in appdata_base v_in);
f2p fragmentShader(in v2f frag_in);
/// A scene SignedDistanceField function, defined in the included shader
SdfResult sdfScene(in float3 p);
SdfResult raymarch(inout Ray3D ray, in int max_distance, in int max_steps, in float epsilon_ray);
float     depthToMaxRayDepth(in float2 screenUV, in float3 rd, in float4x4 inv);
Ray3D     getRayForCamera(float3 screenPos, float3 objectHitpos, float4x4 scale_matrix, float4x4 inv);

#ifndef RAYMARCHING_PARAMETERS
#define RAYMARCHING_PARAMETERS
static float _EPSILON_RAY = 0.0001;
static float _EPSILON_NORMAL = 0.0001;
static float _MAX_DISTANCE = 10000;
static float _RAY_ORIGIN_BIAS;
static float _MAX_STEPS = 500;
UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
#endif

// TODO: use https://computergraphics.stackexchange.com/questions/13666/how-to-calculate-ray-origin-and-ray-direction-in-vertex-shader-working-universal
// to avoid using inverse projection matrix
v2f vertexShader(in appdata_base v_in) {
    v2f o;
    o.vertex = UnityObjectToClipPos(v_in.vertex); // clip space, from (-w,-w,0) to (w, w, w)
    o.screenPos = ComputeScreenPos(o.vertex); // from 0,0 to 1,1
    // o.uv = v.texcoord; // TRANSFORM_TEX(v.texcoord, _BoxmapTex);
    o.hitpos = v_in.vertex;
    // this uses implicitly defined v.vertex.z... possibly migrate to proper function...
    // this: COMPUTE_EYEDEPTH(o.screenPos.z); is replaced with the following to account for struct members:
    o.screenPos.z = -UnityObjectToViewPos(o.vertex).z;
    o.rd_cam = UnityObjectToViewPos(v_in.vertex);
    return o;
}


f2p fragmentShader(in v2f frag_in) {
    float3 clipPos = frag_in.screenPos.xyz / frag_in.screenPos.w; // 0,0 to 1,1 on screen
    float3 rayOrigin, rayDirection;
    Ray3D  ray = (Ray3D)0;
    ray.rd = cameraRayFromClipPos(frag_in.screenPos, ray.ro);
    ray.maxDistance = _MAX_DISTANCE;
    SdfResult sdf = raymarch(ray, _MAX_DISTANCE, _MAX_STEPS, _EPSILON_RAY);

    clip(sdf.id.w); // discard rays without hit

    f2p frag_out = (f2p)0;
    frag_out.color.a = 1;

    frag_out.color = sdf.id == NO_ID ? float4(1.0, 1.0, 0, 1.0) : float4(1.0, 0.1, 0.1, 1.0); // yellow or tomato
    float dist = modulo(ray.marchedDistance, 0.1) / 0.1;
    frag_out.color.rgb = dot(CameraWsForward(), ray.rd) * dist; // color by distance
    fixed4 gridColor = sdf::debug::worldgrid(sdf.p);
    frag_out.color.rgba = gridColor;
    return frag_out;
}

/**
 * TODO: simplify to avoid inverse projection and both rd and uv
 * \brief reads the _CameraDepthTexture (rendered BEFORE this shader!) and returns the distance along rd to reach for that depth
 * \param screenUV a (0,0)->(1,1) screen-space coordinate of the pixel to read depth from
 * \param rd a ray direction from the camera
 * \param inv an inverse projection+world+model* matrix. *depends on the local frame 
 * \return returns a distance along the ray rd to reach for the depth at screenUV
 */
float depthToMaxRayDepth(in float2 screenUV, in float3 rd, in float4x4 inv) {
    // read camera depth texture to correctly blend with scene geometry
    // beware, that _CameraDepthTexture IS NOT the depth buffer!
    // it is populated in the prepass and doesn't change in subsequent passes
    // https://forum.unity.com/threads/does-depth-buffer-update-between-passes.620575/
    float camDepth = CorrectDepth(tex2D(_CameraDepthTexture, screenUV).rg);

    float4 forward = mul(inv, float4(0, 0, 1, 1)); // central ray end on far plane
    forward /= forward.w;
    forward = normalize(forward); // forward in object space
    return camDepth / dot(forward, rd);
}

/**
 * \brief 
 * \param screenPos screen position in (0,0)->(1,1)
 * \param objectHitpos object space mesh hit position
 * \param scale_matrix object scale matrix to handle non-uniform scaling as a way of resizing domain without altering rays
 * \param inv inverse projection+world+model* matrix. *depends on the local frame
 * \return generates ray for a given position. TODO: refactor it to depend only on a screen uv and handle the projection properly 
 */
Ray3D getRayForCamera(float3 screenPos, float3 objectHitpos, float4x4 scale_matrix, float4x4 inv) {
    Ray3D ray = (Ray3D)0;

    // NDC from (-1, -1, -1) to (1, 1, 1) 
    float3 NDC = 2. * screenPos.xyz - 1.;

    float4 ro = mul(inv, float4(NDC.xy, UNITY_NEAR_CLIP_VALUE, 1)); // ray origin on near plane
    ro /= ro.w;
    #ifndef _RAYORIGIN_NEAR
    {
        float4 rs =
            #ifdef _ORIGIN_WORLD
                mul(UNITY_MATRIX_M, float4(objectHitpos, 1));
            #else
            fixed4(objectHitpos, 1);
        #endif
        ray.marchedDistance = distance(mul(rs, scale_matrix), ro); // start on ray
    }
    #endif

    float4 re = mul(inv, float4(NDC.xy, 1, 1)); // ray end on far plane
    re /= re.w;
    float3 rd = normalize((re - ro).xyz); // ray direction

    ray.ro = ro;
    ray.rd = rd;
    ray.maxDistance = depthToMaxRayDepth(screenPos.xy, ray.rd, inv);
    return ray;
}

/**
 * \brief 
 * \param clipPos clip position in range (0,0)->(1,1)
 * \return
 * <a href="https://computergraphics.stackexchange.com/questions/13666/how-to-calculate-ray-origin-and-ray-direction-in-vertex-shader-working-universal">
 * based on my stack exchange question</a>
 */
Ray3D clipPosToWorldSpaceRay(float2 clipPos) {
    Ray3D ray = (Ray3D)0;
    ray.ro = _ProjectionParams.y;
    float3 forward = normalize(UnityWorldSpaceViewDir(float3(0, 0, 0)));
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
SdfResult raymarch(inout Ray3D ray, in int max_distance, in int max_steps, in float epsilon_ray) {
    SdfResult sdf = (SdfResult)0;
    sdf.distance = max_distance;
    sdf.id = int4(NO_ID);

    sdf.material = (Material)0;
    sdf.normal = 0;
    // ray can start inside SDF, this implementation makes it perform one step onto the surface
    // is it good or bad, well, depends on the use case 
    for (ray.steps = 0; ray.steps < max_steps; ray.steps++) {
        if (ray.marchedDistance >= max_distance || ray.marchedDistance >= ray.maxDistance)
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
