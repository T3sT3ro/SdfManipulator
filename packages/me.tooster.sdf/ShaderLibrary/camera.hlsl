#pragma once

#include "UnityCG.cginc"
#include "types.hlsl"

// matrix layout from image
// helpers based on https://github.com/hecomi/uRaymarching/blob/master/Assets/uRaymarching/Runtime/Shaders/Include/Legacy/Camera.cginc
// projection matrix visualization with focal distance: https://3.bp.blogspot.com/-Ta5DyMS7fME/T69ekWPrl-I/AAAAAAAAAVE/GVH43qUzxQw/s1600/projection_matrix.png
// the same thign with near/far values visualized: 
// here a nice visualization showing focal distance and pinhole model: https://www.baeldung.com/cs/focal-length-intrinsic-camera-parameters
/// returns camera position in world space
float3 CameraWsPosition() { return _WorldSpaceCameraPos; }
/// returns camera forward direction in world space
float3 CameraWsForward() { return -UNITY_MATRIX_V[2].xyz; }
/// returns camera right direction in world space
float3 CameraWsRight() { return UNITY_MATRIX_V[0].xyz; }
/// returns camera up direction in world space
float3 CameraWsUp() { return UNITY_MATRIX_V[1].xyz; }
/// returns camera near clip plane value
float  CameraNearClipPlane() { return _ProjectionParams.y; }
/// returns camera far clip plane value
float  CameraFarClipPlane() { return _ProjectionParams.z; }
/// returns focal length of camera, i.e. ctg(fov_y/2)
float  CameraFocalLength() { return abs(UNITY_MATRIX_P[1][1]); }
/// returns width of an orthographic camera
float  CameraOrthoWidth() { return unity_OrthoParams.x; }
/// returns height of an orthographic camera  
float  CameraOrthoHeight() { return unity_OrthoParams.y; }
/// returns 1.0 if camera is orthographic, 0.0 if perspective
float  CameraIsOrtho() { return unity_OrthoParams.w; }
/// returns camera aspect ration (width / height)
float  AspectRatio() { return _ScreenParams.x / _ScreenParams.y; }

/**
 * \brief Returns the world-space ray through a fragment for the current view and projection matrices. The ray starts on
 * the near plane.
 * \param clipPos the clip-space position of the fragment, interpolated from the vertex shader output with perspective
 * correction. Its perspective divide gives the normalized device coordinates of the fragment, also for triangles that
 * cross the camera plane, which per-vertex rays get wrong.
 * \remark It only reads \c UNITY_MATRIX_V and \c UNITY_MATRIX_P, so it works in every pass: in a shadow caster pass
 * they belong to the light, and while the scene view picks objects they cover the picked region. Camera values such as
 * \c _WorldSpaceCameraPos and \c unity_OrthoParams keep the values of the camera there.
 */
Ray3D viewRayFromClipPos(float4 clipPos) {
    float2   ndc = clipPos.xy / clipPos.w;
    float4x4 p = UNITY_MATRIX_P;
    float3   originVs, directionVs;
    // the last row of a perspective projection is (0, 0, -1, 0), and of an orthographic one (0, 0, 0, 1)
    if (p._m33 == 1.0) {
        float nearZ = (UNITY_NEAR_CLIP_VALUE - p._m23) / p._m22;
        originVs = float3((ndc.x - p._m03) / p._m00, (ndc.y - p._m13) / p._m11, nearZ);
        directionVs = float3(0, 0, -1);
    } else {
        float nearZ = -p._m23 / (p._m22 + UNITY_NEAR_CLIP_VALUE);
        directionVs = float3((ndc.x + p._m02) / p._m00, (ndc.y + p._m12) / p._m11, -1); // the point at view depth 1
        originVs = directionVs * -nearZ;
    }

    // the rotation part of a view matrix is orthonormal, so its transpose inverts it
    float3x3 viewToWorld = transpose((float3x3)UNITY_MATRIX_V);
    float3   eyeWs = -mul(viewToWorld, UNITY_MATRIX_V._m03_m13_m23);

    Ray3D ray = (Ray3D)0;
    ray.ro = eyeWs + mul(viewToWorld, originVs);
    ray.rd = normalize(mul(viewToWorld, directionVs));
    return ray;
}
