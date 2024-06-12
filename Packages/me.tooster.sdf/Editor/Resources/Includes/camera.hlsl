#pragma once

#include "UnityCG.cginc"

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
float CameraNearClipPlane() { return _ProjectionParams.y; }
/// returns camera far clip plane value
float CameraFarClipPlane() { return _ProjectionParams.z; }
/// returns focal length of camera, i.e. ctg(fov_y/2)
float CameraFocalLength() { return abs(UNITY_MATRIX_P[1][1]); }
/// returns width of an orthographic camera
float CameraOrthoWidth() { return unity_OrthoParams.x; }
/// returns height of an orthographic camera  
float CameraOrthoHeight() { return unity_OrthoParams.y; }
/// returns 1.0 if camera is orthographic, 0.0 if perspective
float CameraIsOrtho() { return unity_OrthoParams.w; }
/// returns camera aspect ration (width / height)
float AspectRatio() { return _ScreenParams.x / _ScreenParams.y; }

/// generates camera ray direction for a given clip pos
/// based on my question and answer to it:
/// https://computergraphics.stackexchange.com/questions/13666/how-to-calculate-ray-origin-and-ray-direction-in-vertex-shader-working-universal
float3 cameraRayFromClipPos(float4 clipPos, out float3 rayOrigin) {
    clipPos.xy /= clipPos.w; // perspective divide
    clipPos.xy = (clipPos.xy - 0.5) * 2; // transform to origin 0,0 at center and extents of ±1
    clipPos.x *= AspectRatio(); // clipPos becomes x: [-aspectRatio, aspectRatio] and y: [-1, 1]
    // The ray is constructed from a flat (z=0) ray on the near clip plane, and the camera forward vector, i.e. it forms a triangle

    float focalLength = CameraFocalLength();
    if (CameraIsOrtho()) {
        // without focal length divide zoom won't work;
        float3 toNearPlane = (CameraWsForward() * CameraNearClipPlane() + CameraWsRight() * clipPos.x + CameraWsUp() * clipPos.y) / focalLength;
        rayOrigin = CameraWsPosition() + toNearPlane;
        return CameraWsForward();
    } else {
        float3 toNearPlane = CameraWsForward() * focalLength + CameraWsRight() * clipPos.x + CameraWsUp() * clipPos.y;
        rayOrigin = CameraWsPosition() + toNearPlane * CameraNearClipPlane();
        return normalize(toNearPlane);
    }
}

float3 cameraVsRayFromClipPos(float4 clipPos, out float3 rayOrigin) {
    clipPos.xy /= clipPos.w; // perspective divide
    clipPos.xy = (clipPos.xy - 0.5) * 2; // transform to origin 0,0 at center and extents of ±1
    clipPos.x *= AspectRatio(); // clipPos becomes x: [-aspectRatio, aspectRatio] and y: [-1, 1]
    // The ray is constructed from a flat (z=0) ray on the near clip plane, and the camera forward vector, i.e. it forms a triangle

    float focalLength = CameraFocalLength();
    if (CameraIsOrtho()) {
        // without focal length divide zoom won't work;
        float3 toNearPlane = (CameraWsForward() * CameraNearClipPlane() + CameraWsRight() * clipPos.x + CameraWsUp() * clipPos.y) / focalLength;
        rayOrigin = CameraWsPosition() + toNearPlane;
        return CameraWsForward();
    } else {
        float3 toNearPlane = CameraWsForward() * focalLength + CameraWsRight() * clipPos.x + CameraWsUp() * clipPos.y;
        rayOrigin = CameraWsPosition() + toNearPlane * CameraNearClipPlane();
        return toNearPlane;
    }
}
