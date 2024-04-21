#pragma once
#include "util.hlsl"

/**
 * Collection of core primitive 3D SDFs centered at origin.
 * Math and code taken from
 * <a href="https://www.iquilezles.org/www/articles/distfunctions/distfunctions.htm">
 * Inigo Quilez
 * </a>
 *
 * General parameter naming:
 * - <c>p</c>       point in 3D space to calculate SDF value for
 * - <c>R</c>       primary radius
 * - <c>r</c>       secondary radius
 * - <c>dim</c>     dimensions vector
 * - <c>e</c>       extrude
 */
namespace sdf { namespace primitives3D {
    // TODO: dashed line https://www.shadertoy.com/view/Ntyczt

    float sphere(float3 p, float R) {
        return length(p) - R;
    }


    float box(float3 p, float3 dim) {
        float3 v = abs(p) - dim; // bring point to first octant
        return length(max(v, 0.0)) + min(max(v), 0.0);
    }


    float box_frame(float3 p, float3 dim, float extrude) {
        p = abs(p) - dim;
        float3 q = abs(p + extrude) - extrude;
        return min(
            length(max(float3(p.x, q.y, q.z), 0.0)) + min(max(p.x, q.y, q.z), 0.0),
            length(max(float3(q.x, p.y, q.z), 0.0)) + min(max(q.x, p.y, q.z), 0.0),
            length(max(float3(q.x, q.y, p.z), 0.0)) + min(max(q.x, q.y, p.z), 0.0)
        );
    }

    float torus(float3 p, float R, float r) {
        float2 q = float2(length(p.xz) - R, p.y);
        return length(q) - r;
    }

    // FIXME: it "sc" the center or what??? weird rendering artifacts...
    float torus_capped(in float2 p, in float R, in float r, in float2 sc) {
        p.x = abs(p.x);
        float k = (sc.y * p.x > sc.x * p.y) ? dot(p.xy, sc) : length(p.xy);
        return sqrt(dot(p, p) + R * R - 2.0 * R * k) - r;
    }


    float link(float3 p, float l, float R, float r) {
        float3 q = float3(p.x, max(abs(p.y) - l, 0.0), p.z);
        return length(float2(length(q.xy) - R, q.z)) - r;
    }


    float cylinder_infinite(in float3 p, in float r) {
        return length(p.xz) - r;
    }

    float cylinder_capped(in float3 p, in float h, in float r) {
        float2 d = abs(float2(length(p.xz), p.y)) - float2(r, h);
        return min(max(d.x, d.y), 0.0) + length(max(d, 0.0));
    }

    // FIXME: is this needed? round operator should solve it...
    float cylinder_rounded(float3 p, float ra, float rb, float h) {
        float2 d = float2(length(p.xz) - ra + rb, abs(p.y) - h);
        return min(max(d.x, d.y), 0.0) + length(max(d, 0.0)) - rb;
    }

    // plane equation of a plane = xz
    float plane(in float3 p) {
        return p.y;
    }

    float cone(in float3 p, in float angle, in float height) {
        float angle_sin, angle_cos;
        sincos(angle, angle_sin, angle_cos);

        float2 q = height * float2(angle_sin / angle_cos, -1.0);
        float2 w = float2(length(p.xz), p.y);
        float2 a = w - q * clamp(dot(w, q) / dot(q, q), 0.0, 1.0);
        float2 b = w - q * float2(clamp(w.x / q.x, 0.0, 1.0), 1.0);
        float  k = sign(q.y);
        float  d = min(dot(a, a), dot(b, b));
        float  s = max(k * (w.x * q.y - w.y * q.x), k * (w.y - q.y));
        return sqrt(d) * sign(s);
    }
}}
