// GENERATED SHADER CONTENT. ANY MODIFICATIONS WILL BE OVERWRITTEN.
// Last modification: 18/06/2024 22:17:54

Shader "Hello SDF (generated)"
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

        [Header (root combine)]
        SDF_1_0__root_combine__1_BlendFactor ("Blend factor", Float) = 1.0

        [Header (root combine dice)]
        SDF_1_0_0__root_combine_dice__1_BlendFactor ("Blend factor", Float) = 1.0

        [Header (root combine dice box)]
        SDF_1_0_0_0__root_combine_dice_box__2_BoxExtents ("Box extents", Vector) = (0.6, 0.6, 0.601625, 0.0)

        [Header (root combine dice sphere)]
        SDF_1_0_0_1__root_combine_dice_sphere__2_Radius ("Sphere radius", Float) = 0.8

        [Header (root combine cross)]
        SDF_1_0_1__root_combine_cross__1_BlendFactor ("Blend factor", Float) = 1.0

        [Header (root combine cross cylinder)]
        SDF_1_0_1_0__root_combine_cross_cylinder__2_Height ("Cylinder height", Float) = 0.988934
        SDF_1_0_1_0__root_combine_cross_cylinder__2_Radius ("Cylinder radius", Float) = 0.401215
        SDF_1_0_1_0__root_combine_cross_cylinder__2_Rounding ("Rounding", Float) = 0.25

        [Header (root combine cross cylinder)]
        SDF_1_0_1_1__root_combine_cross_cylinder__2_Height ("Cylinder height", Float) = 1.0
        SDF_1_0_1_1__root_combine_cross_cylinder__2_Radius ("Cylinder radius", Float) = 0.402362
        SDF_1_0_1_1__root_combine_cross_cylinder__2_Rounding ("Rounding", Float) = 0.25

        [Header (root combine cross cylinder)]
        SDF_1_0_1_2__root_combine_cross_cylinder__2_Height ("Cylinder height", Float) = 1.0
        SDF_1_0_1_2__root_combine_cross_cylinder__2_Radius ("Cylinder radius", Float) = 0.4
        SDF_1_0_1_2__root_combine_cross_cylinder__2_Rounding ("Rounding", Float) = 0.25
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
        float    SDF_1_0__root_combine__1_BlendFactor;
        float    SDF_1_0_0__root_combine_dice__1_BlendFactor;
        float4x4 SDF_1_0_0_0__root_combine_dice_box__1_SpaceTransform = {
            {1.0, 0.0, 0.0, 0.0},
            {0.0, 1.0, 0.0, 0.0},
            {0.0, 0.0, 1.0, 0.0},
            {0.0, 0.0, 0.0, 1.0}
        };
        float3   SDF_1_0_0_0__root_combine_dice_box__2_BoxExtents;
        float4x4 SDF_1_0_0_1__root_combine_dice_sphere__1_SpaceTransform = {
            {1.0, 0.0, 0.0, 0.0},
            {0.0, 1.0, 0.0, 0.0},
            {0.0, 0.0, 1.0, 0.0},
            {0.0, 0.0, 0.0, 1.0}
        };
        float    SDF_1_0_0_1__root_combine_dice_sphere__2_Radius;
        float    SDF_1_0_1__root_combine_cross__1_BlendFactor;
        float4x4 SDF_1_0_1_0__root_combine_cross_cylinder__1_SpaceTransform = {
            {1.0, 0.0, 0.0, 0.0},
            {0.0, 1.0, 0.0, 0.0},
            {0.0, 0.0, 1.0, 0.0},
            {0.0, 0.0, 0.0, 1.0}
        };
        float    SDF_1_0_1_0__root_combine_cross_cylinder__2_Height;
        float    SDF_1_0_1_0__root_combine_cross_cylinder__2_Radius;
        float    SDF_1_0_1_0__root_combine_cross_cylinder__2_Rounding;
        float4x4 SDF_1_0_1_1__root_combine_cross_cylinder__1_SpaceTransform = {
            {1.0, 0.0, 0.0, 0.0},
            {0.0, 0.0, 1.0, 0.0},
            {0.0, -1.0, 0.0, 0.0},
            {0.0, 0.0, 0.0, 1.0}
        };
        float    SDF_1_0_1_1__root_combine_cross_cylinder__2_Height;
        float    SDF_1_0_1_1__root_combine_cross_cylinder__2_Radius;
        float    SDF_1_0_1_1__root_combine_cross_cylinder__2_Rounding;
        float4x4 SDF_1_0_1_2__root_combine_cross_cylinder__1_SpaceTransform = {
            {0.0, 1.0, 0.0, 0.0},
            {-1.0, 0.0, 0.0, 0.0},
            {0.0, 0.0, 1.0, 0.0},
            {0.0, 0.0, 0.0, 1.0}
        };
        float SDF_1_0_1_2__root_combine_cross_cylinder__2_Height;
        float SDF_1_0_1_2__root_combine_cross_cylinder__2_Radius;
        float SDF_1_0_1_2__root_combine_cross_cylinder__2_Rounding;
        ENDHLSL
        Pass
        {
            HLSLPROGRAM
            SdfResult SDF_1_0_0_0__root_combine_dice_box__3(float3 p);
            SdfResult SDF_1_0_0_1__root_combine_dice_sphere__3(float3 p);
            SdfResult SDF_1_0_0__root_combine_dice__1(float3 p);
            SdfResult SDF_1_0_1_0__root_combine_cross_cylinder__3(float3 p);
            SdfResult SDF_1_0_1_1__root_combine_cross_cylinder__3(float3 p);
            SdfResult SDF_1_0_1_2__root_combine_cross_cylinder__3(float3 p);
            SdfResult SDF_1_0_1__root_combine_cross__1(float3 p);
            SdfResult SDF_1_0__root_combine__1(float3 p);
            SdfResult SDF_1__root__1(float3 p);

            SdfResult SDF_1_0_0_0__root_combine_dice_box__3(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = sdf::primitives3D::box(
                    sdf::operators::transform(p, SDF_1_0_0_0__root_combine_dice_box__1_SpaceTransform), SDF_1_0_0_0__root_combine_dice_box__2_BoxExtents
                );
                result.id = int4(0, 0, 0, 6);
                return result;
            }

            SdfResult SDF_1_0_0_1__root_combine_dice_sphere__3(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = sdf::primitives3D::sphere(
                    sdf::operators::transform(p, SDF_1_0_0_1__root_combine_dice_sphere__1_SpaceTransform), SDF_1_0_0_1__root_combine_dice_sphere__2_Radius
                );
                result.id = int4(0, 0, 0, 9);
                return result;
            }

            SdfResult SDF_1_0_0__root_combine_dice__1(float3 p) {
                SdfResult result = SDF_1_0_0_0__root_combine_dice_box__3(p);
                result = sdf::operators::intersectSimple(result, SDF_1_0_0_1__root_combine_dice_sphere__3(p));
                result.id = int4(0, 0, 0, 3);
                return result;
            }

            SdfResult SDF_1_0_1_0__root_combine_cross_cylinder__3(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = sdf::primitives3D::cylinder_rounded(
                    sdf::operators::transform(p, SDF_1_0_1_0__root_combine_cross_cylinder__1_SpaceTransform), SDF_1_0_1_0__root_combine_cross_cylinder__2_Radius,
                    SDF_1_0_1_0__root_combine_cross_cylinder__2_Rounding, SDF_1_0_1_0__root_combine_cross_cylinder__2_Height
                );
                result.id = int4(0, 0, 0, 13);
                return result;
            }

            SdfResult SDF_1_0_1_1__root_combine_cross_cylinder__3(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = sdf::primitives3D::cylinder_rounded(
                    sdf::operators::transform(p, SDF_1_0_1_1__root_combine_cross_cylinder__1_SpaceTransform), SDF_1_0_1_1__root_combine_cross_cylinder__2_Radius,
                    SDF_1_0_1_1__root_combine_cross_cylinder__2_Rounding, SDF_1_0_1_1__root_combine_cross_cylinder__2_Height
                );
                result.id = int4(0, 0, 0, 16);
                return result;
            }

            SdfResult SDF_1_0_1_2__root_combine_cross_cylinder__3(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = sdf::primitives3D::cylinder_capped(
                    sdf::operators::transform(p, SDF_1_0_1_2__root_combine_cross_cylinder__1_SpaceTransform), SDF_1_0_1_2__root_combine_cross_cylinder__2_Height,
                    SDF_1_0_1_2__root_combine_cross_cylinder__2_Radius
                );
                result.id = int4(0, 0, 0, 19);
                return result;
            }

            SdfResult SDF_1_0_1__root_combine_cross__1(float3 p) {
                SdfResult result = SDF_1_0_1_0__root_combine_cross_cylinder__3(p);
                result = sdf::operators::unionSimple(result, SDF_1_0_1_1__root_combine_cross_cylinder__3(p));
                result = sdf::operators::unionSimple(result, SDF_1_0_1_2__root_combine_cross_cylinder__3(p));
                result.distance *= -1;
                result.id = int4(0, 0, 0, 10);
                return result;
            }

            SdfResult SDF_1_0__root_combine__1(float3 p) {
                SdfResult result = SDF_1_0_0__root_combine_dice__1(p);
                result = sdf::operators::intersectSimple(result, SDF_1_0_1__root_combine_cross__1(p));
                return result;
            }

            SdfResult SDF_1__root__1(float3 p) {
                SdfResult result = SDF_1_0__root_combine__1(p);
                return result;
            }

            SdfResult sdfScene(float3 p) {
                return SDF_1__root__1(p);
            }
            ENDHLSL
        }
    }
}