#pragma once

typedef int ID;
#define NO_ID -1

struct Hit
{
    float distance; // distance from sdf
    ID id; // id of hit object
};

struct RayInfo3D
{
    float3 ro; /// ray origin in world-space
    float3 rd; /// ray direction in world-space
    float3 p; /// mutable 3D point in space where ray is evaluated in world-space
    float3 n; /// normal at point in world-space
    int steps; // how many steps did raymarcher do
    Hit hit; // ray hit results
};


struct v2f
{
    float4 vertex : SV_POSITION; // clip space vertex pos
    float4 screenPos: TEXCOORD1;
    float3 hitpos : TEXCOORD2; // hit position in model space
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