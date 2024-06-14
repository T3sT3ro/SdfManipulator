// GENERATED SHADER CONTENT. ANY MODIFICATIONS WILL BE OVERWRITTEN.
// Last modification: 14/06/2024 20:24:47

Shader "examples (generated)"
{
    Properties
    {
        [Header (global raymarching properties)]
        [Space]
        [Tooltip (turn OFF to raymarch inside object)]
        [Enum (UnityEngine.Rendering.CullMode)]
        _Cull ("Cull", Int) = 0
        [Toggle]
        [KeyEnum (Off, On)]
        [Tooltip (Enable for correct blending with other geometry and backface rendering)]
        _ZWrite ("ZWrite", Float) = 1
        [Enum (UnityEngine.Rendering.CompareFunction)]
        _ZTest ("ZTest", Int) = 4
        [KeywordEnum (Material, Albedo, Id, Skybox, Normal, Steps, Depth, Occlusion)]
        _DrawMode ("Draw mode", Int) = 0
        [Toggle (_SHOW_WORLD_GRID)]
        [Tooltip (Show world grid overlay)]
        _SHOW_WORLD_GRID ("Show World Grid overlay", Float) = 1
        _EPSILON_RAY ("epsilon step for ray to consider hit", Float) = 0.001
        _EPSILON_NORMAL ("epsilon for calculating normal", Float) = 0.001
        _MAX_STEPS ("max raymarching steps", Int) = 200
        _MAX_DISTANCE ("max raymarching distance", Float) = 200.0
        _RAY_ORIGIN_BIAS ("ray origin bias", Float) = 0.0
        _AO_STEPS ("Ambient Occlusion Steps", Int) = 6
        _AO_MAX_DISTANCE ("Ambient Occlusion Max Distance", Float) = 1.0
        _AO_FALLOFF ("Ambient Occlusion Falloff", Float) = 2.5

        [Header (root)]
        SDF_1__root__1_BlendFactor ("Blend factor", Float) = 0.3

        [Header (root allObjects)]
        SDF_1_0__root_allObjects__1_BlendFactor ("Blend factor", Float) = 0.55

        [Header (root allObjects box)]
        SDF_1_0_0__root_allObjects_box__2_BoxExtents ("Box extents", Vector) = (0.25, 0.25, 0.25, 0.0)

        [Header (root allObjects box)]
        SDF_1_0_0__root_allObjects_box__3_Thickness ("Thickness", Float) = 0.32
        SDF_1_0_0__root_allObjects_box__3_Layers ("Layers", Integer) = 5

        [Header (root allObjects toroid)]
        SDF_1_0_1__root_allObjects_toroid__1_BlendFactor ("Blend factor", Float) = 1.0

        [Header (root allObjects toroid torus)]
        SDF_1_0_1_0__root_allObjects_toroid_torus__2_MainRadius ("Torus main radius", Float) = 0.498047
        SDF_1_0_1_0__root_allObjects_toroid_torus__2_RingRadius ("Torus ring radius", Float) = 0.25
        SDF_1_0_1_0__root_allObjects_toroid_torus__2_Cap ("Torus cap", Float) = 0.760655

        [Header (root allObjects toroid torus)]
        SDF_1_0_1_0__root_allObjects_toroid_torus__3_Thickness ("Thickness", Float) = 0.12
        SDF_1_0_1_0__root_allObjects_toroid_torus__3_Layers ("Layers", Integer) = 4

        [Header (root allObjects toroid sphere)]
        SDF_1_0_1_1__root_allObjects_toroid_sphere__2_Radius ("Sphere radius", Float) = 0.995045

        [Header (root allObjects cone)]
        SDF_1_0_2__root_allObjects_cone__2_Angle ("Cone angle", Float) = 0.543944
        SDF_1_0_2__root_allObjects_cone__2_Height ("Cone height", Float) = 1.267364

        [Header (root allObjects sphere)]
        SDF_1_0_3__root_allObjects_sphere__2_Radius ("Sphere radius", Float) = 0.75

        [Header (root allObjects cylinder)]
        SDF_1_0_4__root_allObjects_cylinder__2_Height ("Cylinder height", Float) = 1.0
        SDF_1_0_4__root_allObjects_cylinder__2_Radius ("Cylinder radius", Float) = 0.75
        SDF_1_0_4__root_allObjects_cylinder__2_Rounding ("Rounding", Float) = 0.25

        [Header (root allObjects rounded cylinder)]
        SDF_1_0_5__root_allObjects_rounded_cylinder__2_Height ("Cylinder height", Float) = 0.240286
        SDF_1_0_5__root_allObjects_rounded_cylinder__2_Radius ("Cylinder radius", Float) = 0.75
        SDF_1_0_5__root_allObjects_rounded_cylinder__2_Rounding ("Rounding", Float) = 0.25

        [Header (root allObjects torus)]
        SDF_1_0_6__root_allObjects_torus__2_MainRadius ("Torus main radius", Float) = 0.630713
        SDF_1_0_6__root_allObjects_torus__2_RingRadius ("Torus ring radius", Float) = 0.25
        SDF_1_0_6__root_allObjects_torus__2_Cap ("Torus cap", Float) = 1.738993

        [Header (root cutoff)]
        SDF_1_1__root_cutoff__2_BoxExtents ("Box extents", Vector) = (6.891491, 0.873901, 3.763167, 0.0)
    }
    Fallback "Sdf/Fallback"
    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "Queue" = "Geometry+1"
            "IgnoreProjector" = "True"
            "LightMode" = "ForwardBase"
        }
        ZTest [_ZTest]
        Cull [_Cull]
        ZWrite [_ZWrite]
        HLSLINCLUDE
        #pragma target 5.0
        #include "UnityCG.cginc"
        #include "Packages/me.tooster.sdf/Editor/Resources/Includes/raymarching.hlsl"
        #include "Packages/me.tooster.sdf/Editor/Resources/Includes/util.hlsl"
        #include "Packages/me.tooster.sdf/Editor/Resources/Includes/operators.hlsl"
        #include "Packages/me.tooster.sdf/Editor/Resources/Includes/matrix.hlsl"
        #include "Packages/me.tooster.sdf/Editor/Resources/Includes/primitives.hlsl"
        #include "Packages/me.tooster.sdf/Editor/Resources/Includes/debugBaseShading.hlsl"
        #pragma vertex vertexShader
        #pragma fragment fragmentShader

        float    SDF_1__root__1_BlendFactor;
        float    SDF_1_0__root_allObjects__1_BlendFactor;
        float4x4 SDF_1_0_0__root_allObjects_box__1_SpaceTransform = {
            {1.0, 0.0, 0.0, 0.0},
            {0.0, 1.0, 0.0, -0.25},
            {0.0, 0.0, 1.0, 0.0},
            {0.0, 0.0, 0.0, 1.0}
        };
        float3   SDF_1_0_0__root_allObjects_box__2_BoxExtents;
        float    SDF_1_0_0__root_allObjects_box__3_Thickness;
        int      SDF_1_0_0__root_allObjects_box__3_Layers;
        float    SDF_1_0_1__root_allObjects_toroid__1_BlendFactor;
        float4x4 SDF_1_0_1_0__root_allObjects_toroid_torus__1_SpaceTransform = {
            {0.0, 1.0, 0.0, -0.25},
            {-1.0, 0.0, 0.0, 0.0},
            {0.0, 0.0, 1.0, 2.0},
            {0.0, 0.0, 0.0, 1.0}
        };
        float    SDF_1_0_1_0__root_allObjects_toroid_torus__2_MainRadius;
        float    SDF_1_0_1_0__root_allObjects_toroid_torus__2_RingRadius;
        float    SDF_1_0_1_0__root_allObjects_toroid_torus__2_Cap;
        float    SDF_1_0_1_0__root_allObjects_toroid_torus__3_Thickness;
        int      SDF_1_0_1_0__root_allObjects_toroid_torus__3_Layers;
        float4x4 SDF_1_0_1_1__root_allObjects_toroid_sphere__1_SpaceTransform = {
            {1.0, 0.0, 0.0, -0.684},
            {0.0, 1.0, 0.0, -0.25},
            {0.0, 0.0, 1.0, 2.0},
            {0.0, 0.0, 0.0, 1.0}
        };
        float    SDF_1_0_1_1__root_allObjects_toroid_sphere__2_Radius;
        float4x4 SDF_1_0_2__root_allObjects_cone__1_SpaceTransform = {
            {1.0, 0.0, 0.0, 0.0},
            {0.0, 1.0, 0.0, 0.143},
            {0.0, 0.0, 1.0, 4.0},
            {0.0, 0.0, 0.0, 1.0}
        };
        float    SDF_1_0_2__root_allObjects_cone__2_Angle;
        float    SDF_1_0_2__root_allObjects_cone__2_Height;
        float4x4 SDF_1_0_3__root_allObjects_sphere__1_SpaceTransform = {
            {1.0, 0.0, 0.0, 0.0},
            {0.0, 1.0, 0.0, -0.25},
            {0.0, 0.0, 1.0, 6.0},
            {0.0, 0.0, 0.0, 1.0}
        };
        float    SDF_1_0_3__root_allObjects_sphere__2_Radius;
        float4x4 SDF_1_0_4__root_allObjects_cylinder__1_SpaceTransform = {
            {1.0, 0.0, 0.0, 2.0},
            {0.0, 1.0, 0.0, -0.25},
            {0.0, 0.0, 1.0, -1.495},
            {0.0, 0.0, 0.0, 1.0}
        };
        float    SDF_1_0_4__root_allObjects_cylinder__2_Height;
        float    SDF_1_0_4__root_allObjects_cylinder__2_Radius;
        float    SDF_1_0_4__root_allObjects_cylinder__2_Rounding;
        float4x4 SDF_1_0_5__root_allObjects_rounded_cylinder__1_SpaceTransform = {
            {1.0, 0.0, 0.0, 2.0},
            {0.0, 1.0, 0.0, 0.154},
            {0.0, 0.0, 1.0, 2.0},
            {0.0, 0.0, 0.0, 1.0}
        };
        float    SDF_1_0_5__root_allObjects_rounded_cylinder__2_Height;
        float    SDF_1_0_5__root_allObjects_rounded_cylinder__2_Radius;
        float    SDF_1_0_5__root_allObjects_rounded_cylinder__2_Rounding;
        float4x4 SDF_1_0_6__root_allObjects_torus__1_SpaceTransform = {
            {1.0, 0.0, 0.0, 2.0},
            {0.0, 1.0, 0.0, -0.25},
            {0.0, 0.0, 1.0, 4.0},
            {0.0, 0.0, 0.0, 1.0}
        };
        float    SDF_1_0_6__root_allObjects_torus__2_MainRadius;
        float    SDF_1_0_6__root_allObjects_torus__2_RingRadius;
        float    SDF_1_0_6__root_allObjects_torus__2_Cap;
        float4x4 SDF_1_1__root_cutoff__1_SpaceTransform = {
            {1.0, 0.0, 0.0, 0.0},
            {0.0, 1.0, 0.0, -1.214},
            {0.0, 0.0, 1.0, 2.0},
            {0.0, 0.0, 0.0, 1.0}
        };
        float3 SDF_1_1__root_cutoff__2_BoxExtents;
        ENDHLSL
        Pass
        {
            HLSLPROGRAM
            SdfResult SDF_1_0_0__root_allObjects_box__4(float3 p);
            SdfResult SDF_1_0_1_0__root_allObjects_toroid_torus__4(float3 p);
            SdfResult SDF_1_0_1_1__root_allObjects_toroid_sphere__3(float3 p);
            SdfResult SDF_1_0_1__root_allObjects_toroid__1(float3 p);
            SdfResult SDF_1_0_2__root_allObjects_cone__3(float3 p);
            SdfResult SDF_1_0_3__root_allObjects_sphere__3(float3 p);
            SdfResult SDF_1_0_4__root_allObjects_cylinder__3(float3 p);
            SdfResult SDF_1_0_5__root_allObjects_rounded_cylinder__3(float3 p);
            SdfResult SDF_1_0_6__root_allObjects_torus__3(float3 p);
            SdfResult SDF_1_0__root_allObjects__1(float3 p);
            SdfResult SDF_1_1__root_cutoff__3(float3 p);
            SdfResult SDF_1__root__1(float3 p);

            SdfResult SDF_1_0_0__root_allObjects_box__4(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = sdf::operators::onion_skin(
                    sdf::primitives3D::box(sdf::operators::transform(p, SDF_1_0_0__root_allObjects_box__1_SpaceTransform), SDF_1_0_0__root_allObjects_box__2_BoxExtents),
                    SDF_1_0_0__root_allObjects_box__3_Thickness, SDF_1_0_0__root_allObjects_box__3_Layers
                );
                result.id = int4(0, 0, 0, 6);
                return result;
            }

            SdfResult SDF_1_0_1_0__root_allObjects_toroid_torus__4(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = sdf::operators::onion_skin(
                    sdf::primitives3D::torus(
                        sdf::operators::transform(p, SDF_1_0_1_0__root_allObjects_toroid_torus__1_SpaceTransform), SDF_1_0_1_0__root_allObjects_toroid_torus__2_MainRadius,
                        SDF_1_0_1_0__root_allObjects_toroid_torus__2_RingRadius
                    ), SDF_1_0_1_0__root_allObjects_toroid_torus__3_Thickness, SDF_1_0_1_0__root_allObjects_toroid_torus__3_Layers
                );
                result.id = int4(0, 0, 0, 11);
                return result;
            }

            SdfResult SDF_1_0_1_1__root_allObjects_toroid_sphere__3(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = sdf::primitives3D::sphere(
                    sdf::operators::transform(p, SDF_1_0_1_1__root_allObjects_toroid_sphere__1_SpaceTransform), SDF_1_0_1_1__root_allObjects_toroid_sphere__2_Radius
                );
                result.id = int4(0, 0, 0, 14);
                return result;
            }

            SdfResult SDF_1_0_1__root_allObjects_toroid__1(float3 p) {
                SdfResult result = SDF_1_0_1_0__root_allObjects_toroid_torus__4(p);
                result = sdf::operators::intersectSimple(result, SDF_1_0_1_1__root_allObjects_toroid_sphere__3(p));
                return result;
            }

            SdfResult SDF_1_0_2__root_allObjects_cone__3(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = sdf::primitives3D::cone(
                    (sdf::operators::transform(p, SDF_1_0_2__root_allObjects_cone__1_SpaceTransform) - float3(0, SDF_1_0_2__root_allObjects_cone__2_Height, 0)),
                    SDF_1_0_2__root_allObjects_cone__2_Angle, SDF_1_0_2__root_allObjects_cone__2_Height
                );
                result.id = int4(0, 0, 0, 17);
                return result;
            }

            SdfResult SDF_1_0_3__root_allObjects_sphere__3(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = sdf::primitives3D::sphere(
                    sdf::operators::transform(p, SDF_1_0_3__root_allObjects_sphere__1_SpaceTransform), SDF_1_0_3__root_allObjects_sphere__2_Radius
                );
                result.id = int4(0, 0, 0, 20);
                return result;
            }

            SdfResult SDF_1_0_4__root_allObjects_cylinder__3(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = sdf::primitives3D::cylinder_capped(
                    sdf::operators::transform(p, SDF_1_0_4__root_allObjects_cylinder__1_SpaceTransform), SDF_1_0_4__root_allObjects_cylinder__2_Height,
                    SDF_1_0_4__root_allObjects_cylinder__2_Radius
                );
                result.id = int4(0, 0, 0, 23);
                return result;
            }

            SdfResult SDF_1_0_5__root_allObjects_rounded_cylinder__3(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = sdf::primitives3D::cylinder_rounded(
                    sdf::operators::transform(p, SDF_1_0_5__root_allObjects_rounded_cylinder__1_SpaceTransform), SDF_1_0_5__root_allObjects_rounded_cylinder__2_Radius,
                    SDF_1_0_5__root_allObjects_rounded_cylinder__2_Rounding, SDF_1_0_5__root_allObjects_rounded_cylinder__2_Height
                );
                result.id = int4(0, 0, 0, 26);
                return result;
            }

            SdfResult SDF_1_0_6__root_allObjects_torus__3(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = sdf::primitives3D::torus_capped(
                    sdf::operators::transform(p, SDF_1_0_6__root_allObjects_torus__1_SpaceTransform), SDF_1_0_6__root_allObjects_torus__2_MainRadius,
                    SDF_1_0_6__root_allObjects_torus__2_RingRadius, SDF_1_0_6__root_allObjects_torus__2_Cap
                );
                result.id = int4(0, 0, 0, 29);
                return result;
            }

            SdfResult SDF_1_0__root_allObjects__1(float3 p) {
                SdfResult result = SDF_1_0_0__root_allObjects_box__4(p);
                result = sdf::operators::unionSimple(result, SDF_1_0_1__root_allObjects_toroid__1(p));
                result = sdf::operators::unionSimple(result, SDF_1_0_2__root_allObjects_cone__3(p));
                result = sdf::operators::unionSimple(result, SDF_1_0_3__root_allObjects_sphere__3(p));
                result = sdf::operators::unionSimple(result, SDF_1_0_4__root_allObjects_cylinder__3(p));
                result = sdf::operators::unionSimple(result, SDF_1_0_5__root_allObjects_rounded_cylinder__3(p));
                result = sdf::operators::unionSimple(result, SDF_1_0_6__root_allObjects_torus__3(p));
                return result;
            }

            SdfResult SDF_1_1__root_cutoff__3(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = -sdf::primitives3D::box(sdf::operators::transform(p, SDF_1_1__root_cutoff__1_SpaceTransform), SDF_1_1__root_cutoff__2_BoxExtents);
                result.id = int4(0, 0, 0, 32);
                return result;
            }

            SdfResult SDF_1__root__1(float3 p) {
                SdfResult result = SDF_1_0__root_allObjects__1(p);
                result = sdf::operators::intersectSimple(result, SDF_1_1__root_cutoff__3(p));
                return result;
            }

            SdfResult sdfScene(float3 p) {
                return SDF_1__root__1(p);
            }
            ENDHLSL
        }
    }
}