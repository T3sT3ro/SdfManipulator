#pragma once
#include <HLSLSupport.cginc>
// Upgrade NOTE: excluded shader from DX11; has structs without semantics (struct v2f members distance)
// fixme: why was this added again??? I don't remember
// #pragma exclude_renderers d3d11

// this kind of ID helps when doing domain repetition
typedef fixed4 ID;
#define NO_ID int4(-1, -1, -1, -1) ///< sucky cocky

/// basic material model for SDFs TODO: migrate to official shading material properties for unity
struct Material {
    fixed4 albedo; ///< base diffuse color + alpha (in diffuse or specular workflow)
    half   metallic; ///< 0=non-metal, 1=metal
    half   smoothness; ///< 0=rough, 1=smooth
    half3  emission; ///< "glow" without bleeding to other objects 
    half   occlusion; ///< occlusion (default 1). How much indirect light should surface receive. 1 is full, 0 is none (e.g. in creases)
};

/// Result of a raymarching operation
struct SdfResult {
    ID id; ///< id of a hit object. (-1, -1, -1, -1) if not hit. XYZ can be used in domain repetition, and w do teremine unique primitives
    // TODO: maybe remove this and expose it as *out* parameter wherever it's needed?
    float3   p; ///< 3D point where SDF was evaluated
    float    distance; ///< result of the SDF at point \p p
    fixed3   normal; ///< optional; tangent space normal, if written
    Material material; ///< optional; if defined, material of asurface at this point. Can be a blend of many materials.
};

/// A ray metadata for a 3D space tightly coupled with a raymarcher. TODO: split into Ray (ro, rd) and RaymarchInfo (maxDistance, marchedDistance, steps)
struct Ray3D {
    float3 ro; ///< ray origin in world-space
    float3 rd; ///< ray direction in world-space
    int    steps; ///< how many steps did raymarcher do
    float  marchedDistance; ///< distance marched by raymarcher in world-space units
    float  maxDistance; ///< maximum distance a raymarcher is able to march for that ray
};

/// data passed from vertex to fragment shader
struct v2f {
    /// clip space vertex pos. Space in vertex shader: (-w, -w, 0)->(w,w,w). In fragment (after hardware perspective divide) is (-1, -1, 0*) to (1,1,1*). Z value depends on platform.
    float4 vertex : SV_POSITION;
    float4 screenPos: TEXCOORD1; ///< clip-pos vertex position. This is not perspective divided, so that `w` is not lost beetween stages
    float3 hitpos : TEXCOORD2; ///< hit position in model space TODO: remove
    float3 rd_cam : TEXCOORD3; ///< ray direction in camera space TODO: use to offload ray calculation to vertex shader
};

/// result of a fragment shader 
struct f2p {
    fixed4 color: SV_Target0; ///< fragment colro output
    float3 normal: SV_Target1; ///< fragment's world normal
    ID     ID: SV_Target2; ///< ID of hit object, auxiliary data used for picking or domain repetition
    #ifdef _ZWRITE_ON
    float depth: SV_Depth; ///< depth value of the fragment when the raymarcher has to modify depth.
    #endif
};
