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
    int steps; // how many steps did raymarcher do
    Hit hit; // ray hit results
};


struct v2f
{
    float4 vertex : SV_POSITION; // clip space vertex pos
    float3 o_ok_ro : TEXCOORD0; // world space ray origin - start of frustum
    float3 o_ok_re : TEXCOORD1; // world space ray end - end of frustum
    
    float3 p_ok_ro : TEXCOORD2; // ray origin
    float3 p_ok_hit : TEXCOORD3; // ray's hit position on mesh
};

struct f2p
{
    fixed4 color: SV_Target0;
    float3 normal: SV_Target1; // world normal
    ID ID: SV_Target2; // ID of object, for picking
};