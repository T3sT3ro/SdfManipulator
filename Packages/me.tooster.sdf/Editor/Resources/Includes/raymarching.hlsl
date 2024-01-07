#pragma once
#include "UnityCG.cginc"
#include "util.hlsl"
#include "Packages/me.tooster.sdf/Editor/Resources/Includes/types.hlsl"

// TODO: use https://computergraphics.stackexchange.com/questions/13666/how-to-calculate-ray-origin-and-ray-direction-in-vertex-shader-working-universal
// to avoid using inverse projection matrix
v2f vert(appdata_base v)
{
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex); // clip space
    o.screenPos = ComputeScreenPos(o.vertex); // from 0,0 to 1,1
    // o.uv = v.texcoord; // TRANSFORM_TEX(v.texcoord, _BoxmapTex);
    o.hitpos = v.vertex;
    // this uses implicitly defined v.vertex.z... possibly migrate to proper function...
    COMPUTE_EYEDEPTH(o.screenPos.z);
    o.rd_cam = UnityObjectToViewPos(v.vertex);
    return o;
}

UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);

float depthToMaxRayDepth(in float2 screenUV, in float3 rd, in float4x4 inv)
{
    // read camera depth texture to correctly blend with scene geometry
    // beware, that _CameraDepthTexture IS NOT the depth buffer!
    // it is populated in the prepass and doesn't change in subsequent passes
    // https://forum.unity.com/threads/does-depth-buffer-update-between-passes.620575/
    float camDepth = CorrectDepth(tex2D(_CameraDepthTexture, screenUV).rg);

    float4 forward = mul(inv, float4(0, 0, 1, 1)); // ray end on far plane
    forward /= forward.w;
    forward = normalize(forward); // forward in object space
    return camDepth / dot(forward, rd);
}

Ray3D getRaysForCamera(float3 screenPos, float3 objectHitpos, float4x4 scale_matrix, float4x4 inv)
{
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
        ray.startDistance = distance(mul(rs, scale_matrix), ro); // start on ray
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
