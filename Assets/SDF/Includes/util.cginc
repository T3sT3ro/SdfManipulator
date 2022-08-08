#pragma once

/**
 * This file includes common utility functions for writing more conscience code
 */

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

// rotations from https://www.pouet.net/topic.php?which=7931&page=1&x=3&y=14

void rotX(inout float3 p, float a)
{
    float c, s;
    float3 q = p;
    sincos(a, s, c);
    p.y = c * q.y - s * q.z;
    p.z = s * q.y + c * q.z;
}

void rotY(inout float3 p, float a)
{
    float c, s;
    float3 q = p;
    sincos(a, s, c);
    p.x = c * q.x + s * q.z;
    p.z = -s * q.x + c * q.z;
}

void rotZ(inout float3 p, float a)
{
    float c, s;
    float3 q = p;
    sincos(a, s, c);
    p.x = c * q.x - s * q.y;
    p.y = s * q.x + c * q.y;
}

// overloads below are used for explicit [c]os and [s]in
void rotXCS(inout float3 p, float c, float s)
{
    float3 q = p;
    p.y = c * q.y - s * q.z;
    p.z = s * q.y + c * q.z;
}

void rotYCS(inout float3 p, float c, float s)
{
    float3 q = p;
    p.x = c * q.x + s * q.z;
    p.z = -s * q.x + c * q.z;
}

void rotZCS(inout float3 p, float c, float s)
{
    float3 q = p;
    p.x = c * q.x - s * q.y;
    p.y = s * q.x + c * q.y;
}

float square(float rise, float fall, float t)
{
    return step(rise, t) * step(t, fall);
}

// returns correct depth in both persp and ortho
// from https://forum.unity.com/threads/depth-buffer-with-orthographic-camera.355878/
// from https://forum.unity.com/threads/getting-scene-depth-z-buffer-of-the-orthographic-camera.601825/#post-4966334

float CorrectDepth(float rawDepth)
{
    float persp = LinearEyeDepth(rawDepth);
    float ortho = (_ProjectionParams.z - _ProjectionParams.y) * rawDepth + _ProjectionParams.y;
    return lerp(persp, ortho, unity_OrthoParams.w);
}