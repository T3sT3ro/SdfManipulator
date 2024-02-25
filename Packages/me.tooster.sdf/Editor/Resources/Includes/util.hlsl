#pragma once
#include <UnityCG.cginc>

#include "types.hlsl"

/**
 * This file includes common utility functions for writing more conscience code
 */

Material lerpMaterials(Material m1, Material m2, float t) {
    Material mixed;
    mixed.albedo = lerp(m1.albedo, m2.albedo, t);
    mixed.emission = lerp(m1.emission, m2.emission, t);
    mixed.metallic = lerp(m1.metallic, m2.metallic, t);
    mixed.smoothness = lerp(m1.smoothness, m2.smoothness, t);
    mixed.occlusion = lerp(m1.occlusion, m2.occlusion, t);
    return mixed;
}

#define MODULO(TV, n, TM) TV##n modulo(in TV##n v, in TM mod) { return v - mod * floor(v / mod); }
/** returns mathematically correct modulo */
MODULO(float, 1, float)
/** returns mathematically correct modulo */
MODULO(float, 2, float)
/** returns mathematically correct modulo */
MODULO(float, 3, float)
/** returns mathematically correct modulo */
MODULO(float, 4, float)
#undef MODULO

/// for an array of values returns lerp between two adjacent values 
#define LEVELS(levels, t) lerp(levels[floor(t)], levels[ceil(t)], frac(t))

// 3 component utils
float max(in float3 v) { return max(v.x, max(v.y, v.z)); }
float max(in float a, in float b, in float c) { return max(a, max(b, c)); }
float min(in float3 v) { return min(v.x, min(v.y, v.z)); }
float min(in float a, in float b, in float c) { return min(a, min(b, c)); }
// 4 component utils
float max(in float4 v) { return max(max(v.x, v.y), max(v.z, v.w)); }
float max(in float a, in float b, in float c, in float d) { return max(max(a, b), max(c, d)); }
float min(in float4 v) { return min(min(v.x, v.y), min(v.z, v.w)); }
float min(in float a, in float b, in float c, in float d) { return min(min(a, b), min(c, d)); }

// https://blog.demofox.org/2016/02/19/normalized-vector-interpolation-tldr/
float3 slerp(in float3 start, in float3 end, in float t) {
    float slerpDot = dot(start, end);
    slerpDot = clamp(slerpDot, -1.0, 1.0); // clamp into acos range
    float  theta = acos(slerpDot) * t;
    float3 basis = normalize(end - start * slerpDot); // Orthonormal basis
    return start * cos(theta) + basis * sin(theta);
}

// rotations from https://www.pouet.net/topic.php?which=7931&page=1&x=3&y=14

void rotX(inout float3 p, float a) {
    float  c, s;
    float3 q = p;
    sincos(a, s, c);
    p.y = c * q.y - s * q.z;
    p.z = s * q.y + c * q.z;
}

void rotY(inout float3 p, float a) {
    float  c, s;
    float3 q = p;
    sincos(a, s, c);
    p.x = c * q.x + s * q.z;
    p.z = -s * q.x + c * q.z;
}

void rotZ(inout float3 p, float a) {
    float  c, s;
    float3 q = p;
    sincos(a, s, c);
    p.x = c * q.x - s * q.y;
    p.y = s * q.x + c * q.y;
}

// overloads below are used for explicit [c]os and [s]in
void rotXCS(inout float3 p, float c, float s) {
    float3 q = p;
    p.y = c * q.y - s * q.z;
    p.z = s * q.y + c * q.z;
}

void rotYCS(inout float3 p, float c, float s) {
    float3 q = p;
    p.x = c * q.x + s * q.z;
    p.z = -s * q.x + c * q.z;
}

void rotZCS(inout float3 p, float c, float s) {
    float3 q = p;
    p.x = c * q.x - s * q.y;
    p.y = s * q.x + c * q.y;
}

float square(float rise, float fall, float t) {
    return step(rise, t) * step(t, fall);
}

// returns correct depth in both persp and ortho
// from https://forum.unity.com/threads/depth-buffer-with-orthographic-camera.355878/
// from https://forum.unity.com/threads/getting-scene-depth-z-buffer-of-the-orthographic-camera.601825/#post-4966334

float CorrectDepth(float rawDepth) {
    float persp = LinearEyeDepth(rawDepth);
    float ortho = (_ProjectionParams.z - _ProjectionParams.y) * rawDepth + _ProjectionParams.y;
    return lerp(persp, ortho, unity_OrthoParams.w);
}

// should be an inverse function of CorrectDepth() above
float EncodeCorrectDepth(float eyeDepth) {
    float persp = (1.0 / eyeDepth - _ZBufferParams.w) / _ZBufferParams.z;
    float ortho = (eyeDepth - _ProjectionParams.y) / (_ProjectionParams.z - _ProjectionParams.y);
    return lerp(persp, ortho, unity_OrthoParams.w);
}
