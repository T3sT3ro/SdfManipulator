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
    float3 ro; // ray origin
    float3 rd; // ray direction
    float3 p; // mutable 3D point in space where ray is evaluated
    float3 n; // normal at point
    int steps; // how many steps did raymarcher do
    Hit hit; // ray hit results
};


struct v2f
{
    float4 vertex : SV_POSITION; // clip space vertex pos
    float4 screenPos: TEXCOORD1;
    float2 uv : TEXCOORD2; // UV of the model
    float3 hitpos : TEXCOORD3; // hit position in model space
};

struct f2p
{
    fixed4 color: SV_Target0;
    float3 normal: SV_Target1; // world normal
    ID ID: SV_Target2; // ID of object, for picking
    // #ifdef _WRITE_DEPTH
    // float depth: SV_Depth;
    // #endif
};