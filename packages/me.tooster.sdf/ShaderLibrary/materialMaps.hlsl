#pragma once

/*
 * Texture maps of SDF materials. The materials are evaluated once at the hit, in SdfMaterial, which sets the shading
 * point that the maps read.
 */


/**
 * \brief Biplanar mapping: samples \p tex on the two planes that face \p n most and blends them, which needs two samples
 * instead of the three of tri-planar mapping.
 * \param k sharpness of the blend; 8 is a good default
 * \remarks based on <a href="https://iquilezles.org/articles/biplanar/">IQ's biplanar mapping</a>, without texture gradients
 */
float4 biplanar(Texture2D tex, SamplerState s, float3 p, float3 n, float k) {
    n = abs(n);
    // the axes in decreasing order of how much the surface faces them
    int3 major = n.x > n.y && n.x > n.z ? int3(0, 1, 2) : n.y > n.z ? int3(1, 2, 0) : int3(2, 0, 1);
    int3 minor = n.x < n.y && n.x < n.z ? int3(0, 1, 2) : n.y < n.z ? int3(1, 2, 0) : int3(2, 0, 1);
    int3 median = int3(3, 3, 3) - minor - major;
    float4 x = tex.SampleLevel(s, float2(p[major.y], p[major.z]), 0);
    float4 y = tex.SampleLevel(s, float2(p[median.y], p[median.z]), 0);
    float2 w = float2(n[major.x], n[median.x]);
    w = saturate((w - 0.5773) / (1 - 0.5773));
    w = pow(w, k / 8.0);
    return (x * w.x + y * w.y) / (w.x + w.y);
}

/// The surface point and normal of the current shading in world space. SdfMaterial sets them before it evaluates the
/// materials of the scene; marching doesn't read the materials, so it doesn't need them.
static float3 sdfShadingPosition;
static float3 sdfShadingNormal;
SamplerState  sampler_linear_repeat;

/// Samples \p map with biplanar mapping at the shading point, in the space of \p mapSpace, with one repetition every
/// \p size units of that space
float4 sampleShadingMap(Texture2D map, float4x4 mapSpace, float size) {
    float3 p = mul(mapSpace, float4(sdfShadingPosition, 1)).xyz / size;
    float3 n = mul((float3x3)mapSpace, sdfShadingNormal);
    return biplanar(map, sampler_linear_repeat, p, n, 8);
}
