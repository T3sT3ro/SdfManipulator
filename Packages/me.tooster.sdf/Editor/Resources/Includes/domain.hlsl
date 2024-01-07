#pragma once
/*
 * This file provides means to abstract away the implementation of domain origin and rescale
 * the intended coordinate system to perform calculations would be world
 */

#include "UnityCG.cginc"
#include "matrix.hlsl"

#pragma shader_feature_local _ORIGIN_WORLD _ORIGIN_LOCAL
#pragma shader_feature_local _PRESERVE_SPACE_SCALE_ON

CBUFFER_START(SDF_domain)
float3 _DomainOrigin;
float3 _DomainRotation;
float3 _DomainScale;
CBUFFER_END

static const float4x4 _SDF_MatrixSpaceRescale =
    #if defined(_PRESERVE_SPACE_SCALE_ON) && defined(_ORIGIN_LOCAL)
            extract_scale_matrix(UNITY_MATRIX_M);
    #else
    MATRIX_ID;
#endif

// used to premultiply before object transformation
static const float4x4 _SDF_MatrixSpaceRescaleInv = inverse_diagonal(_SDF_MatrixSpaceRescale);

static const float4x4 _SDF_MatrixDomainRotation = mul(m_scale(_DomainScale), _SDF_MatrixSpaceRescaleInv);
static const float4x4 _SDF_MatrixDomainScale = mul(m_scale(_DomainScale), _SDF_MatrixSpaceRescaleInv);
static const float4x4 _SDF_MatrixDomainTranslation = mul(m_scale(_DomainScale), _SDF_MatrixSpaceRescaleInv);

// takes care of local local object scale
static const float4x4 _SDF_MatrixDomainToObject =
    #if defined(_ORIGIN_LOCAL)
    mul(_SDF_MatrixSpaceRescaleInv, mul(_SDF_MatrixDomainTranslation, mul(_SDF_MatrixDomainScale, _SDF_MatrixDomainRotation)));
    #else
    mul(unity_WorldToObject, mul(_SDF_MatrixDomainTranslation, mul(_SDF_MatrixDomainScale, _SDF_MatrixDomainRotation)));
#endif

static const float4x4 _SDF_MatrixObjectToDomain = inverse(_SDF_MatrixDomainToObject);

static const float4x4 _SDF_MatrixDomainToWorld =
    #if defined(_ORIGIN_WORLD)
    mul(_SDF_MatrixDomainTranslation, mul(_SDF_MatrixDomainScale, _SDF_MatrixDomainRotation));
    #else
    mul(unity_ObjectToWorld, _SDF_MatrixDomainToObject);
#endif

static const float4x4 _SDF_MatrixWorldToDomain = inverse(_SDF_MatrixDomainToWorld);

// Domain <-> Object
float4 SdfDomainToObject(float4 pos)
{
    return mul(_SDF_MatrixDomainToObject, pos);
}

float3 SDFDomainToObjectDir(float3 dir)
{
    return mul((float3x3)_SDF_MatrixDomainToObject, dir);
}

float4 SDFDomainToObjectNormal(float3 norm)
{
    return mul(norm, _SDF_MatrixObjectToDomain);
}

// Domain <-> World

float4 SDFDomainToWorldPos(float4 pos)
{
    return mul(_SDF_MatrixDomainToWorld, pos);
}

float3 SDFDomainToWorldPos(float3 pos)
{
    return SDFDomainToWorldPos(float4(pos, 1.0)).xyz;
}
