#pragma once

/**
 * Collection of operators for 3D SDFs centered at origin.
 * Math and code taken from
 * <a href="https://www.iquilezles.org/www/articles/distfunctions/distfunctions.htm">
 * Inigo Quilez
 * </a>
 *
 * 
 * General parameter naming:
 * - <c>p</c>           point in 3D space before it is evaluated by the SDF function.
 * - <c>distance</c>    distance returned by the evaluated SDF. 
 * - <c>R</c>           primary radius
 * - <c>r</c>           secondary radius
 * - <c>dim</c>         dimensions vector for X,Y,Z sizes
 * - <c>transform</c>   rigidbody(!) transformation matrix
 */
namespace sdf
{
    namespace operators
    {
        // smooth minimum with blending factor k
        // result is returned in x and blending factor in y 
        // from inigo quilez: https://iquilezles.org/articles/smin/
        float2 smin(float a, float b, float k)
        {
            float h = max(k - abs(a - b), 0.0) / k;
            float m = h * h * 0.5;
            float s = m * k * (1.0 / 2.0);
            return a < b ? float2(a - s, m) : float2(b - s, 1.0 - m);
        }

        float3 transform(float3 p, float4x4 invTransform)
        {
            return mul(invTransform, float4(p, 1));
        }

        float round_sdf(in float distance, float R)
        {
            return distance - R;
        }

        /**
         * repeat in period of c units in each axis 
         */
        float3 repeat(in float3 p, in float3 c)
        {
            return fmod(abs(p) + 0.5 * c, c) - 0.5 * c;
        }

        float3 repeatLim(in float3 p, in float3 c, in float3 l, out int3 index)
        {
            index = clamp(round(p / c), -l, l);
            return p - c * index;
        }

        /**
         * Elongate the primitive. This is a prefix operation that transforms point passed to primitive.
         * This method produces exact 1D elongations but produces kernel of 0 distances inside 2D/3D elongations.
         */
        float elongate_fast(float3 p, float3 dim)
        {
            return p - clamp(p, -dim, dim);
        }

        /**
         * Elongate the primitive. This is a prefix operation that transforms point passed to primitive.
         * This is the exact method 
         */
        // float elongate_exact(float3 p, float3 dim)
        // {
        //     float3 q = abs(p) - dim;
        //     return __SDF(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0);
        // }
    }
}
