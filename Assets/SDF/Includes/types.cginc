#pragma once

typedef int ID;
#define NO_ID -1

struct Hit
{
    float distance;
    ID id;
};

struct RayInfo3D
{
    float3 ro;
    float3 rd;
    float3 p; /// 3D point in space where ray is evaluated
    int steps; /// how many steps did raymarcher do
    Hit hit; /// ray hit results
};

struct appdata
{
    float4 vertex : POSITION;
    float2 uv: TEXCOORD0;
};

struct v2f
{
    float4 vertex : SV_POSITION; // world space vertex pos
    float3 ro : TEXCOORD0; // ray origin
    float3 hitpos : TEXCOORD1; // ray's hit position on mesh
};

struct f2p
{
    fixed4 color: SV_Target0;
    float3 normal: SV_Target1;
    ID ID: SV_Target2;
    float depth: SV_Depth;
};