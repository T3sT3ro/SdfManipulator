#pragma once
// Upgrade NOTE: excluded shader from DX11; has structs without semantics (struct v2f members distance)
#pragma exclude_renderers d3d11

// this kind of ID helps when doing domain repetition
typedef fixed4 ID;
#define NO_ID int4(-1, -1, -1, -1)

struct Material
{
    fixed4 albedo;      // base (diffuse or specular) color + alpha
    half metallic;      // 0=non-metal, 1=metal
    half smoothness;    // 0=rough, 1=smooth
    half3 emission;
    half occlusion;     // occlusion (default 1)
};

struct SdfResult
{
    ID id; // id of hit object
    float3 p; /// mutable 3D point in space where ray is evaluated in world-space
    float distance; // distance from sdf
    fixed3 normal;      // tangent space normal, if written
    Material material;
};

struct Ray3D
{
    float3 ro; /// ray origin in world-space
    float3 rd; /// ray direction in world-space
    int steps; // how many steps did raymarcher do
    float startDistance;
    float maxDistance;
};

struct v2f
{
    float4 vertex : SV_POSITION; // clip space vertex pos
    float4 screenPos: TEXCOORD1;
    float3 hitpos : TEXCOORD2; // hit position in model space
    float3 rd_cam : TEXCOORD3; // ray direction in camera space
};

struct f2p
{
    fixed4 color: SV_Target0;
    float3 normal: SV_Target1; // world normal
    ID ID: SV_Target2; // ID of object, for picking
    #ifdef _ZWRITE_ON
    float depth: SV_Depth;
    #endif
};

/// data structure for 3d boxmap textures and sizing, separate for each axis
struct BoxMapParams3D
{
    sampler2D X;
    sampler2D Y;
    sampler2D Z;

    float4 X_ST;
    float4 Y_ST;
    float4 Z_ST;
};