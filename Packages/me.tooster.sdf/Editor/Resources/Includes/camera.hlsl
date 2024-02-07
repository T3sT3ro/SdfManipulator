#pragma once

#include "UnityCG.cginc"

// matrix layout from image
// helpers based on https://github.com/hecomi/uRaymarching/blob/master/Assets/uRaymarching/Runtime/Shaders/Include/Legacy/Camera.cginc
/// returns camera position in world space
float3 CameraWsPosition() { return UNITY_MATRIX_I_V._14_24_34; }
/// returns camera forward direction in world space
float3 CameraWsForward() { return -UNITY_MATRIX_V[2].xyz; }
/// returns camera right direction in world space
float3 CameraWsRight() { return UNITY_MATRIX_V[0].xyz; }
/// returns camera up direction in world space
float3 CameraWsUp() { return UNITY_MATRIX_V[1].xyz; }
// returns camera near clip plane value
float CameraNearClipPlane() { return _ProjectionParams.y; }
// returns camera far clip plane value
float CameraFarClipPlane() { return _ProjectionParams.z; }
// returns focal length of camera
float CameraFocalLength() { return abs(UNITY_MATRIX_P[1][1]); }
/// returns width of an orthographic camera
float CameraOrthoWidth() { return unity_OrthoParams.x; }
/// returns height of an orthographic camera  
float CameraOrthoHeight() { return unity_OrthoParams.y; }
/// returns 1.0 if camera is orthographic, 0.0 if perspective
float CameraIsOrtho() { return unity_OrthoParams.x; }
/// returns camera aspect ration (width / height)
float AspectRatio() { return _ScreenParams.x / _ScreenParams.y; }

/// generates camera ray direction for a given clip pos
/// based on my question and answer to it:
/// https://computergraphics.stackexchange.com/questions/13666/how-to-calculate-ray-origin-and-ray-direction-in-vertex-shader-working-universal
float3 cameraRayFromClipPos(float4 clipPos, out float3 rayOrigin, out float3 rayDirection)
{
    clipPos.xy /= clipPos.w; // perspective divide
    clipPos.xy = clipPos.xy * 0.5 + 0.5; // transform to origin 0,0 at center and extents of ±0.5
    clipPos.x *= AspectRatio(); // y is the normalized unit size height
    // The ray is constructed from a flat ray on the clip near plane and the forward vector. TODO: account for orthographic camera
    return normalize(CameraWsForward() * CameraFocalLength() + CameraWsRight() * clipPos.x + CameraWsUp() * clipPos.y);
}
