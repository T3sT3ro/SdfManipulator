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
float max(in float a, in float b, in float c, in float d) { return max(max (a, b), max(c, d)); }
float min(in float4 v) { return min(min(v.x, v.y), min(v.z, v.w)); }
float min(in float a, in float b, in float c, in float d) { return min(min (a, b), min(c, d)); }