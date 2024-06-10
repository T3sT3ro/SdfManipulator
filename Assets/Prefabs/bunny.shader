// GENERATED SHADER CONTENT. ANY MODIFICATIONS WILL BE OVERWRITTEN.
// Last modification: 10/06/2024 16:39:14

Shader "bunny (generated)"
{
    Properties
    {
        [Header (global raymarching properties)]
        [Space]
        [Enum (UnityEngine.Rendering.CullMode)]
        _Cull ("Cull", Int) = 2
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

        [Header (root head)]
        SDF_1_0__root_head__1_BlendFactor ("Blend factor", Float) = 1.0

        [Header (root head face)]
        SDF_1_0_0__root_head_face__2_Radius ("Sphere radius", Float) = 0.329848

        [Header (root head face)]
        SDF_1_0_0__root_head_face__3_Length ("Elongation", Vector) = (0.55, 0.5, -0.12, 0.0)

        [Header (root head q)]
        SDF_1_0_1__root_head_q__2_BoxExtents ("Box extents", Vector) = (0.25, 0.25, 0.25, 0.0)

        [Header (root head q)]
        SDF_1_0_1__root_head_q__3_Angle ("Cone angle", Float) = 0.785398
        SDF_1_0_1__root_head_q__3_Height ("Cone height", Float) = 1.0

        [Header (root head q)]
        SDF_1_0_1__root_head_q__4_Length ("Elongation", Vector) = (0.47, -0.17, 0.0, 0.0)

        [Header (root head q)]
        SDF_1_0_2__root_head_q__2_BoxExtents ("Box extents", Vector) = (0.25, 0.25, 0.25, 0.0)

        [Header (root head q)]
        SDF_1_0_2__root_head_q__3_Length ("Elongation", Vector) = (0.47, -0.17, 0.0, 0.0)
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
        float    SDF_1_0__root_head__1_BlendFactor;
        float4x4 SDF_1_0_0__root_head_face__1_SpaceTransform = {
            {1.0, 0.0, 0.0, -0.176},
            {0.0, 1.0, 0.0, 0.0},
            {0.0, 0.0, 1.0, 0.527},
            {0.0, 0.0, 0.0, 1.0}
        };
        float    SDF_1_0_0__root_head_face__2_Radius;
        float3   SDF_1_0_0__root_head_face__3_Length;
        float4x4 SDF_1_0_1__root_head_q__1_SpaceTransform = {
            {0.98226, -0.143563, 0.120642, -0.206409},
            {0.121016, 0.976741, 0.177008, -1.017869},
            {-0.143248, -0.159269, 0.976787, 0.209946},
            {0.0, 0.0, 0.0, 1.0}
        };
        float3   SDF_1_0_1__root_head_q__2_BoxExtents;
        float    SDF_1_0_1__root_head_q__3_Angle;
        float    SDF_1_0_1__root_head_q__3_Height;
        float3   SDF_1_0_1__root_head_q__4_Length;
        float4x4 SDF_1_0_2__root_head_q__1_SpaceTransform = {
            {0.98226, -0.143563, 0.120642, -0.206409},
            {0.121016, 0.976741, 0.177008, -1.017869},
            {-0.143248, -0.159269, 0.976787, 0.209946},
            {0.0, 0.0, 0.0, 1.0}
        };
        float3 SDF_1_0_2__root_head_q__2_BoxExtents;
        float3 SDF_1_0_2__root_head_q__3_Length;
        ENDHLSL
        Pass
        {
            HLSLPROGRAM
            float     SDF_1_0_0__root_head_face__3(float3 p);
            SdfResult SDF_1_0_0__root_head_face__4(float3 p);
            float     SDF_1_0_1__root_head_q__4(float3 p);
            SdfResult SDF_1_0_1__root_head_q__5(float3 p);
            float     SDF_1_0_2__root_head_q__3(float3 p);
            SdfResult SDF_1_0_2__root_head_q__4(float3 p);
            SdfResult SDF_1_0__root_head__1(float3 p);
            SdfResult SDF_1__root__1(float3 p);

            float SDF_1_0_0__root_head_face__3(float3 p) {
                float3 q = abs(p) - SDF_1_0_0__root_head_face__3_Length;
                return sdf::primitives3D::sphere(max(q, 0), SDF_1_0_0__root_head_face__2_Radius) + min(max(q), 0);
            }

            SdfResult SDF_1_0_0__root_head_face__4(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = SDF_1_0_0__root_head_face__3(sdf::operators::transform(p, SDF_1_0_0__root_head_face__1_SpaceTransform));
                result.id = int4(0, 0, 0, 6);
                return result;
            }

            float SDF_1_0_1__root_head_q__4(float3 p) {
                float3 q = abs(p) - SDF_1_0_1__root_head_q__4_Length;
                return sdf::primitives3D::cone(
                    (max(q, 0) - float3(0, SDF_1_0_1__root_head_q__3_Height, 0)), SDF_1_0_1__root_head_q__3_Angle, SDF_1_0_1__root_head_q__3_Height
                ) + min(max(q), 0);
            }

            SdfResult SDF_1_0_1__root_head_q__5(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = SDF_1_0_1__root_head_q__4(sdf::operators::transform(p, SDF_1_0_1__root_head_q__1_SpaceTransform));
                result.id = int4(0, 0, 0, 11);
                return result;
            }

            float SDF_1_0_2__root_head_q__3(float3 p) {
                float3 q = abs(p) - SDF_1_0_2__root_head_q__3_Length;
                return sdf::primitives3D::box(max(q, 0), SDF_1_0_2__root_head_q__2_BoxExtents) + min(max(q), 0);
            }

            SdfResult SDF_1_0_2__root_head_q__4(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = SDF_1_0_2__root_head_q__3(sdf::operators::transform(p, SDF_1_0_2__root_head_q__1_SpaceTransform));
                result.id = int4(0, 0, 0, 15);
                return result;
            }

            SdfResult SDF_1_0__root_head__1(float3 p) {
                SdfResult result = SDF_1_0_0__root_head_face__4(p);
                result = sdf::operators::unionSimple(result, SDF_1_0_1__root_head_q__5(p));
                result = sdf::operators::unionSimple(result, SDF_1_0_2__root_head_q__4(p));
                return result;
            }

            SdfResult SDF_1__root__1(float3 p) {
                SdfResult result = SDF_1_0__root_head__1(p);
                return result;
            }

            SdfResult sdfScene(float3 p) {
                return SDF_1__root__1(p);
            }
            ENDHLSL
        }
    }
}