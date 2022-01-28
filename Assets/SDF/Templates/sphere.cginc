#pragma once
#include "Assets/SDF/Includes/core.cginc"
#include "Assets/SDF/Includes/primitives.cginc"

_OUT float TEMPLATE(_IN float3 p, _IN float3 R)
{
    _BODY sdf::primitives3D::sphere(p, R);
}
