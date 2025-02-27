#pragma once
#include "raymarching.hlsl"

/**
 * Collection of operators for 3D SDFs centered at origin.
 * Math and code based on
 * <a href="https://www.iquilezles.org/www/articles/distfunctions/distfunctions.htm">
 * Inigo Quilez
 * </a>
 *
 * 
 * General parameter naming:
 * - \p p           point in 3D space before it is evaluated by the SDF function.
 * - \p distance    distance returned by the evaluated SDF. 
 * - \p R           primary radius
 * - \p r           secondary radius
 * - \p dim         dimensions vector for X,Y,Z sizes
 * - \p transform   rigidbody(!) transformation matrix
 */
namespace sdf { namespace operators {
    // smooth minimum with blending factor k
    // result is returned in x and blending factor in y 
    // from inigo quilez: https://iquilezles.org/articles/smin/
    float smin(in float a, in float b, in float k, out float blend_factor) {
        float h = max(k - abs(a - b), 0.0) / k;
        float m = h * h * 0.5;
        float s = m * k * (1.0 / 2.0);
        if (a < b) {
            blend_factor = m;
            return a - s;
        } else {
            blend_factor = 1.0 - m;
            return b - s;
        }
    }

    float3 transform(float3 p, float4x4 invTransform) {
        return mul(invTransform, float4(p, 1));
    }

    float distanceUnion(in float3 a, in float3 b) {
        return min(a, b);
    }

    float distanceIntersect(in float3 a, in float3 b) {
        return max(a, b);
    }

    float distanceSubtract(in float3 a, in float3 b) {
        return distanceIntersect(a, -b);
    }

    SdfResult unionSimple(in SdfResult a, in SdfResult b) {
        // this is dumb, but returning in ternary produces GL error about type mismatch
        // Also returning in the middle of the body produces warning of "uninitialized variable"...
        SdfResult result = b;
        if (a.distance < b.distance)
            result = a;
        return result;
    }

    SdfResult intersectSimple(in SdfResult a, in SdfResult b) {
        SdfResult result;
        if (a.distance > b.distance) // this is dumb, but ternary produces GL error about type mismatch
            result = a;
        else
            result = b;
        return result;
    }

    SdfResult unionSmooth(SdfResult a, SdfResult b, float k) {
        SdfResult result;
        float     f;
        result.distance = smin(a.distance, b.distance, k, f);
        result.normal = normalize(lerp(a.normal, b.normal, f));
        result.id = f <= 0.5 ? a.id : b.id;
        result.material = lerpMaterials(a.material, b.material, f);
        result.p = a.p; // due to this, remove `p` from `SdfResult`
        return result;
    }

    float round_sdf(in float distance, float R) {
        return distance - R;
    }

    /**
     * repeat in period of c units in each axis 
     */
    float3 repeat(in float3 p, in float3 c) {
        return fmod(abs(p) + 0.5 * c, c) - 0.5 * c;
    }

    float3 repeatLim(in float3 p, in float3 c, in float3 l, out int3 index) {
        index = clamp(round(p / c), -l, l);
        return p - c * index;
    }

    /**
     * Elongate the primitive. This is a prefix operation that transforms point passed to primitive.
     * This method produces exact 1D elongations but produces kernel of 0 distances inside 2D/3D elongations.
     */
    float elongate_fast(in float3 p, in float3 dim) {
        return p - clamp(p, -dim, dim);
    }

    /**
     * Exact elongation of primitive. This is a circumfix operation that transforms point passed to primitive.
     * Impossible to implement generically and requires hardcoded SDF function, because it does postprocess(SDF(preoprocess(p))).
     */
    // float elongate_exact(float3 p, float3 dim)
    // {
    //     float3 q = abs(p) - dim;
    //     return SDF(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0);
    // }

    /**
     * Calculates the onion skin effect for a given distance and thickness.
     *
     * @param distance The distance to apply the onion skin effect to.
     * @param thickness The thickness of the onion skin layer.
     *
     * @return The resulting distance after applying the onion skin effect.
     */
    float onion_skin(in float distance, in float thickness) {
        return abs(distance) - thickness;
    }

    /**
     * Calculates the repeated onion skin effect for a given distance and number of layers.
     *
     * @param distance The distance to apply the onion skin effect to.
     * @param thickness The thickness of each onion skin layer.
     * @param layers The number of onion skin layers to apply.
     *
     * @return The resulting distance after applying the repeated onion skin effect.
     */
    float onion_skin(in float distance, in float thickness, in int layers) {
        for (int i = 0; i < layers; i++) {
            distance = onion_skin(distance, thickness);
            thickness /= 2;
        }
        return distance;
    }

    /**
     * Interpolates between two distances based on a given t value.
     *
     * @param d1 First distance.
     * @param d2 Second distance.
     * @param t The interpolation factor between 0 and 1.
     *
     * @return The interpolated distance value.
     */
    float interpolate(in float d1, in float d2, in float t) {
        return lerp(d1, d2, t);
    }

    /// Bend space by factor k. Some artifacts are present if k is too big.
    float3 cheap_bend(in float3 p, in float k) {
        float    c = cos(k * p.x);
        float    s = sin(k * p.x);
        float2x2 m = float2x2(c, -s, s, c);
        return float3(mul(m, p.xy), p.z);
    }

    /**
     * "Kink" operator, a distance semi-preserving alternative to "bend" to introduce an "elbow bend" effect (i.e. localized bend) around center.
     * @remark basd on CC0 code from https://www.shadertoy.com/view/3llfRl
     * @param p The input 2D vector representing a point in space.
     * @param k A factor that controls the amount of kink or bend. A value of 1 means no kink, while lower values increase the kink.
     * @returns The transformed 2D vector with the "kink" applied.
     */
    float3 kink(float3 p, float k) {
        // Convert to polar coordinates in the xy plane
        float ang = atan2(p.y, p.x); // Note: atan2 is used for correct angle calculation
        float len = length(float2(p.x, p.y));

        // Warp angle with sigmoid function
        ang -= ang / sqrt(1.0 + ang * ang) * (1.0 - k);

        // Convert back to cartesian coordinates
        float2 rotated = float2(sin(ang), cos(ang)) * len;

        // Return the transformed vector with the rotated xy components and unchanged z component
        return float3(rotated.x, rotated.y, p.z);
    }
}}
