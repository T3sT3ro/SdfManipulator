#pragma once
#pragma exclude_renderers d3d11
#include <HLSLSupport.cginc>

/// Data structure for 3d boxmap parameters, determining a sampler, a texture, and texture parameters. 
struct BoxMapParams {
    SamplerState s; ///< sampler state for sampling attached texture
    Texture2D    tex; ///< texture
    float4       st; ///< texture tiling (xy) and offset (zw)
};

/**
 * Performs a box mapping using 3 textures for each axis.
 * @param x_params parameters for the x axis
 * @param y_params parameters for the y axis
 * @param z_params parameters for the z axis
 * @param p position
 * @param n normal
 * @param k blending factor
 * @returns the boxmapped color
 */
fixed4 trimap(in BoxMapParams x_params, in BoxMapParams y_params, in BoxMapParams z_params, in float3 p, in float3 n, in float k) {
    fixed4 x = x_params.tex.Sample(x_params.s, p.zy * x_params.st.xy + x_params.st.zw);
    fixed4 y = y_params.tex.Sample(y_params.s, p.zx * y_params.st.xy + y_params.st.zw);
    fixed4 z = z_params.tex.Sample(z_params.s, p.xy * z_params.st.xy + z_params.st.zw);

    float3 w = pow(abs(n), k);
    return (x * w.x + y * w.y + z * w.z) / (w.x + w.y + w.z);
}

fixed4 trimap(in BoxMapParams params, in float3 p, in float3 n, in float k) {
    return trimap(params, params, params, p, n, k);
}
