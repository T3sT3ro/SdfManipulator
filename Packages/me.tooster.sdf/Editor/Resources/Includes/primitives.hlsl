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
namespace sdf
{
    namespace primitives3D
    {
        // TODO: dashed line https://www.shadertoy.com/view/Ntyczt
        //

        float sphere(float3 p, float R)
        {
            return length(p) - R;
        }


        float box(float3 p, float3 dim)
        {
            float3 v = abs(p) - dim; // bring point to first octant
            return length(max(v, 0.0)) + min(max(v), 0.0);
        }


        float box_frame(float3 p, float3 dim, float extrude)
        {
            p = abs(p) - dim;
            float3 q = abs(p + extrude) - extrude;
            return min(
                length(max(float3(p.x, q.y, q.z), 0.0)) + min(max(p.x, q.y, q.z), 0.0),
                length(max(float3(q.x, p.y, q.z), 0.0)) + min(max(q.x, p.y, q.z), 0.0),
                length(max(float3(q.x, q.y, p.z), 0.0)) + min(max(q.x, q.y, p.z), 0.0)
            );
        }

        float torus(float3 p, float R, float r)
        {
            float2 q = float2(length(p.xz) - R, p.y);
            return length(q) - r;
        }


        float torus_capped(in float2 p, in float2 sc, in float R, in float r)
        {
            p.x = abs(p.x);
            float k = (sc.y * p.x > sc.x * p.y) ? dot(p.xy, sc) : length(p.xy);
            return sqrt(dot(p, p) + R * R - 2.0 * R * k) - r;
        }


        float link(float3 p, float l, float R, float r)
        {
            float3 q = float3(p.x, max(abs(p.y) - l, 0.0), p.z);
            return length(float2(length(q.xy) - R, q.z)) - r;
        }


        float infinite_cylinder(float3 p, float3 r)
        {
            return length(p.xz - r.xy) - r.z;
        }

        // plane equation as n.xyz := plane normal, n.w := distance along normal
        float plane(float3 p, float4 n)
        {
            n = normalize(n);
            return dot(p, n.xyz) + n.w;
        }
    }
}
