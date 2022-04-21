#pragma once

/**
 * This file includes common utility functions for writing more conscience code
 */

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

// cheap noise
float rand(float2 p) { return frac(sin(dot(p.xy, float2(12.9898, 78.233))) * 43758.5453); }

// rotations from https://www.pouet.net/topic.php?which=7931&page=1&x=3&y=14

void rX(inout float3 p, float a) {
 float c,s; float3 q=p;
 sincos(a, s, c);
 p.y = c * q.y - s * q.z;
 p.z = s * q.y + c * q.z;
}

void rY(inout float3 p, float a) {
 float c,s;float3 q=p;
 sincos(a, s, c);
 p.x = c * q.x + s * q.z;
 p.z = -s * q.x + c * q.z;
}

void rZ(inout float3 p, float a) {
 float c,s;float3 q=p;
 sincos(a, s, c);
 p.x = c * q.x - s * q.y;
 p.y = s * q.x + c * q.y;
}
void rXCS(inout float3 p, float c, float s) {
 float3 q=p;
 p.y = c * q.y - s * q.z;
 p.z = s * q.y + c * q.z;
}


void rYCS(inout float3 p, float c, float s) {
 float3 q=p;
 p.x = c * q.x + s * q.z;
 p.z = -s * q.x + c * q.z;
}

void rZCS(inout float3 p, float c, float s) {
 float3 q=p;
 p.x = c * q.x - s * q.y;
 p.y = s * q.x + c * q.y;
}
