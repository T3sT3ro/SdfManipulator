#pragma once

void random_float(in float2 st, out float v) {
    v = frac(
        sin(
            dot(
                st.xy,
                float2(12.9898, 78.233)
            )
        ) *
        43758.5453123
    );
}

// Based on Morgan McGuire @morgan3d
// https://www.shadertoy.com/view/4dS3Wd
void noise_float(in float2 st, out float v) {
    float2 i = floor(st);
    float2 f = frac(st);

    // Four corners in 2D of a tile
    float a, b, c, d;
    random_float(i, a);
    random_float(i + float2(1.0, 0.0), b);
    random_float(i + float2(0.0, 1.0), c);
    random_float(i + float2(1.0, 1.0), d);

    float2 u = f * f * (3.0 - 2.0 * f);

    v = lerp(a, b, u.x) +
        (c - a) * u.y * (1.0 - u.x) +
        (d - b) * u.x * u.y;
}

void fbm_float(in float2 st, in int octaves, out float v) {
    v = 0.0;
    float  a = 0.5;
    float2 shift = float2(100.0, 100.0);
    // Rotate to reduce axial bias
    float2x2 rot = float2x2(
        cos(0.5), sin(0.5),
        -sin(0.5), cos(0.50)
    );
    for (int i = 0; i < octaves; ++i) {
        float tmpv;
        noise_float(st, tmpv);
        v += a * tmpv;
        st = mul(st, rot) * 2.0 + shift;
        a *= 0.5;
    }
}
