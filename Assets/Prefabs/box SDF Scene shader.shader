// GENERATED SHADER CONTENT. ANY MODIFICATIONS WILL BE OVERWRITTEN.
// Last modification: 07/06/2024 22:48:06

Shader "Box SDF Scene (generated)"
{
    Properties
    {
        [Header (global_raymarching_properties)]
        [Space]
        [Enum (UnityEngine.Rendering.CullMode)]
        _Cull ("Cull", Int) = 2
        [Tooltip (Enable for correct blending with other geometry and backface rendering)]
        [Toggle]
        [KeyEnum (Off, On)]
        _ZWrite ("ZWrite", Float) = 1
        [Enum (UnityEngine.Rendering.CompareFunction)]
        _ZTest ("ZTest", Int) = 4

        [Header (root)]
        SDF_1__root__1_BlendFactor ("Blend factor", Float) = 0.58

        [Header (root_ground)]
        SDF_1_0__root_ground__2_BoxExtents ("Box extents", Vector) = (6.442449, 0.169183, 3.634491, 0.0)

        [Header (root_shroom_1_sphere_1)]
        SDF_1_1_0__root_shroom_1_sphere_1__2_Radius ("Sphere radius", Float) = 0.582681

        [Header (root_shroom_1_cone)]
        SDF_1_1_1__root_shroom_1_cone__2_Angle ("Cone angle", Float) = 0.979781
        SDF_1_1_1__root_shroom_1_cone__2_Height ("Cone height", Float) = 0.59053

        [Header (root_shroom2_sphere_2)]
        SDF_1_2_0__root_shroom2_sphere_2__2_Radius ("Sphere radius", Float) = 0.264981

        [Header (root_shroom2_cone_2)]
        SDF_1_2_1__root_shroom2_cone_2__2_Angle ("Cone angle", Float) = 0.656545
        SDF_1_2_1__root_shroom2_cone_2__2_Height ("Cone height", Float) = 1.02

        [Header (root_shroom2_sphere)]
        SDF_1_2_2__root_shroom2_sphere__2_Radius ("Sphere radius", Float) = 0.339796

        [Header (root_combine)]
        SDF_1_3__root_combine__1_BlendFactor ("Blend factor", Float) = 0.608677

        [Header (root_combine_sphere)]
        SDF_1_3_0__root_combine_sphere__2_Radius ("Sphere radius", Float) = 0.538078

        [Header (root_combine_sphere)]
        SDF_1_3_1__root_combine_sphere__2_Radius ("Sphere radius", Float) = 0.422096

        [Header (root_screw_cylinder)]
        SDF_1_4_0__root_screw_cylinder__2_Height ("Cylinder height", Float) = 1.399991
        SDF_1_4_0__root_screw_cylinder__2_Radius ("Cylinder radius", Float) = 0.404426
        SDF_1_4_0__root_screw_cylinder__2_Rounding ("Rounding", Float) = 0.25

        [Header (root_screw_head)]
        SDF_1_4_1__root_screw_head__1_BlendFactor ("Blend factor", Float) = 1.0

        [Header (root_screw_head_head)]
        SDF_1_4_1_0__root_screw_head_head__2_Radius ("Sphere radius", Float) = 0.53

        [Header (root_screw_head_cuts)]
        SDF_1_4_1_1__root_screw_head_cuts__1_BlendFactor ("Blend factor", Float) = 1.0

        [Header (root_screw_head_cuts_cut_1)]
        SDF_1_4_1_1_0__root_screw_head_cuts_cut_1__2_BoxExtents ("Box extents", Vector) = (0.413049, 0.589912, 0.117837, 0.0)

        [Header (root_screw_head_cuts_cut_2)]
        SDF_1_4_1_1_1__root_screw_head_cuts_cut_2__2_BoxExtents ("Box extents", Vector) = (0.696923, 0.091037, 0.585179, 0.0)

        [Header (root_q)]
        SDF_1_5__root_q__1_BlendFactor ("Blend factor", Float) = 1.0

        [Header (root_q_cylinder)]
        SDF_1_5_0__root_q_cylinder__2_Height ("Cylinder height", Float) = 1.442987
        SDF_1_5_0__root_q_cylinder__2_Radius ("Cylinder radius", Float) = 0.558908
        SDF_1_5_0__root_q_cylinder__2_Rounding ("Rounding", Float) = 0.11

        [Header (root_q_torus)]
        SDF_1_5_1__root_q_torus__2_MainRadius ("Torus main radius", Float) = 0.859511
        SDF_1_5_1__root_q_torus__2_RingRadius ("Torus ring radius", Float) = 0.461763
        SDF_1_5_1__root_q_torus__2_Cap ("Torus cap", Vector) = (0.53, -0.18, 0.0, 0.0)

        [Header (root_flat__1_)]
        SDF_1_6__root_flat__1___1_BlendFactor ("Blend factor", Float) = 1.0

        [Header (root_flat__1__cylinder)]
        SDF_1_6_0__root_flat__1__cylinder__2_Height ("Cylinder height", Float) = 1.426067
        SDF_1_6_0__root_flat__1__cylinder__2_Radius ("Cylinder radius", Float) = 0.73139
        SDF_1_6_0__root_flat__1__cylinder__2_Rounding ("Rounding", Float) = 0.0

        [Header (root_flat__1__torus)]
        SDF_1_6_1__root_flat__1__torus__2_MainRadius ("Torus main radius", Float) = 0.859511
        SDF_1_6_1__root_flat__1__torus__2_RingRadius ("Torus ring radius", Float) = 0.232351
        SDF_1_6_1__root_flat__1__torus__2_Cap ("Torus cap", Vector) = (0.53, -0.18, 0.0, 0.0)

        [Header (root_two_cones)]
        SDF_1_7__root_two_cones__1_BlendFactor ("Blend factor", Float) = 0.46

        [Header (root_two_cones_cone)]
        SDF_1_7_0__root_two_cones_cone__2_Angle ("Cone angle", Float) = 0.46199
        SDF_1_7_0__root_two_cones_cone__2_Height ("Cone height", Float) = 1.258141

        [Header (root_two_cones_cone)]
        SDF_1_7_1__root_two_cones_cone__2_Angle ("Cone angle", Float) = 0.518797
        SDF_1_7_1__root_two_cones_cone__2_Height ("Cone height", Float) = 1.751412
    }
    Fallback "Sdf/Fallback"
    CustomEditor "me.tooster.sdf.Editor.Controllers.Editors.SdfShaderEditor"
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
        #pragma vertex vertexShader
        #pragma fragment fragmentShader

        float    SDF_1__root__1_BlendFactor;
        float4x4 SDF_1_0__root_ground__1_SpaceTransform = {
            {0.985253, 0.039483, 0.166489, 0.363901},
            {-0.049748, 0.99708, 0.057943, -0.130124},
            {-0.163715, -0.065371, 0.98434, 0.229378},
            {0.0, 0.0, 0.0, 1.0}
        };
        float3   SDF_1_0__root_ground__2_BoxExtents;
        float4x4 SDF_1_1_0__root_shroom_1_sphere_1__1_SpaceTransform = {
            {0.970696, -0.101283, 0.217924, -1.013172},
            {0.128052, 0.985376, -0.112415, -0.629878},
            {-0.203352, 0.137026, 0.96947, -0.008134},
            {0.0, 0.0, 0.0, 1.0}
        };
        float    SDF_1_1_0__root_shroom_1_sphere_1__2_Radius;
        float4x4 SDF_1_1_1__root_shroom_1_cone__1_SpaceTransform = {
            {0.939308, 0.069367, -0.335989, -0.946175},
            {-0.061828, 0.997538, 0.033097, -1.137951},
            {0.337457, -0.010315, 0.941284, -0.489848},
            {0.0, 0.0, 0.0, 1.0}
        };
        float    SDF_1_1_1__root_shroom_1_cone__2_Angle;
        float    SDF_1_1_1__root_shroom_1_cone__2_Height;
        float4x4 SDF_1_2_0__root_shroom2_sphere_2__1_SpaceTransform = {
            {-0.02342, 0.103423, -0.994362, -1.404643},
            {0.097179, 0.99016, 0.100697, -0.47743},
            {0.994991, -0.094273, -0.03324, 1.135447},
            {0.0, 0.0, 0.0, 1.0}
        };
        float    SDF_1_2_0__root_shroom2_sphere_2__2_Radius;
        float4x4 SDF_1_2_1__root_shroom2_cone_2__1_SpaceTransform = {
            {-0.773348, 0.262247, -0.5772, -1.871582},
            {0.241606, 0.963641, 0.114114, -0.697886},
            {0.586139, -0.051205, -0.808591, -0.364815},
            {0.0, 0.0, 0.0, 1.0}
        };
        float    SDF_1_2_1__root_shroom2_cone_2__2_Angle;
        float    SDF_1_2_1__root_shroom2_cone_2__2_Height;
        float4x4 SDF_1_2_2__root_shroom2_sphere__1_SpaceTransform = {
            {-0.212623, 0.246355, -0.945569, -1.822549},
            {0.23447, 0.952287, 0.195382, -1.333403},
            {0.948586, -0.180165, -0.260241, 0.84638},
            {0.0, 0.0, 0.0, 1.0}
        };
        float    SDF_1_2_2__root_shroom2_sphere__2_Radius;
        float    SDF_1_3__root_combine__1_BlendFactor;
        float4x4 SDF_1_3_0__root_combine_sphere__1_SpaceTransform = {
            {-0.763447, -0.052902, 0.088036, -1.502791},
            {0.064521, 0.266704, 0.719795, -0.023237},
            {-0.079912, 0.720743, -0.259892, -0.671471},
            {0.0, 0.0, 0.0, 1.0}
        };
        float    SDF_1_3_0__root_combine_sphere__2_Radius;
        float4x4 SDF_1_3_1__root_combine_sphere__1_SpaceTransform = {
            {-0.763447, -0.052902, 0.088036, -2.080791},
            {0.064521, 0.266704, 0.719795, -0.154237},
            {-0.079912, 0.720743, -0.259892, -0.671471},
            {0.0, 0.0, 0.0, 1.0}
        };
        float    SDF_1_3_1__root_combine_sphere__2_Radius;
        float4x4 SDF_1_4_0__root_screw_cylinder__1_SpaceTransform = {
            {0.347078, -0.299924, 0.888585, -0.455998},
            {-0.301492, 0.861506, 0.408546, -2.532097},
            {-0.888054, -0.409698, 0.208585, -2.923648},
            {0.0, 0.0, 0.0, 1.0}
        };
        float    SDF_1_4_0__root_screw_cylinder__2_Height;
        float    SDF_1_4_0__root_screw_cylinder__2_Radius;
        float    SDF_1_4_0__root_screw_cylinder__2_Rounding;
        float    SDF_1_4_1__root_screw_head__1_BlendFactor;
        float4x4 SDF_1_4_1_0__root_screw_head_head__1_SpaceTransform = {
            {0.355245, -0.296137, 0.886625, -0.429045},
            {-0.301492, 0.861506, 0.408546, -4.122097},
            {-0.884818, -0.412444, 0.216763, -2.927725},
            {0.0, 0.0, 0.0, 1.0}
        };
        float    SDF_1_4_1_0__root_screw_head_head__2_Radius;
        float    SDF_1_4_1_1__root_screw_head_cuts__1_BlendFactor;
        float4x4 SDF_1_4_1_1_0__root_screw_head_cuts_cut_1__1_SpaceTransform = {
            {0.301492, -0.861506, -0.408546, 4.421096},
            {0.355245, -0.296137, 0.886625, -0.429044},
            {-0.884818, -0.412444, 0.216763, -2.927725},
            {0.0, 0.0, 0.0, 1.0}
        };
        float3   SDF_1_4_1_1_0__root_screw_head_cuts_cut_1__2_BoxExtents;
        float4x4 SDF_1_4_1_1_1__root_screw_head_cuts_cut_2__1_SpaceTransform = {
            {0.301492, -0.861506, -0.408546, 4.550097},
            {0.355245, -0.296137, 0.886625, -0.429044},
            {-0.884818, -0.412444, 0.216763, -2.927725},
            {0.0, 0.0, 0.0, 1.0}
        };
        float3   SDF_1_4_1_1_1__root_screw_head_cuts_cut_2__2_BoxExtents;
        float    SDF_1_5__root_q__1_BlendFactor;
        float4x4 SDF_1_5_0__root_q_cylinder__1_SpaceTransform = {
            {1.0, 0.0, 0.0, -3.582294},
            {0.0, 1.0, 0.0, -1.580709},
            {0.0, 0.0, 1.0, -3.369244},
            {0.0, 0.0, 0.0, 1.0}
        };
        float    SDF_1_5_0__root_q_cylinder__2_Height;
        float    SDF_1_5_0__root_q_cylinder__2_Radius;
        float    SDF_1_5_0__root_q_cylinder__2_Rounding;
        float4x4 SDF_1_5_1__root_q_torus__1_SpaceTransform = {
            {1.0, 0.0, 0.0, -3.582294},
            {0.0, 1.0, 0.0, -2.343709},
            {0.0, 0.0, 1.0, -3.369244},
            {0.0, 0.0, 0.0, 1.0}
        };
        float    SDF_1_5_1__root_q_torus__2_MainRadius;
        float    SDF_1_5_1__root_q_torus__2_RingRadius;
        float2   SDF_1_5_1__root_q_torus__2_Cap;
        float    SDF_1_6__root_flat__1___1_BlendFactor;
        float4x4 SDF_1_6_0__root_flat__1__cylinder__1_SpaceTransform = {
            {1.0, 0.0, 0.0, -2.150593},
            {0.0, 1.0, 0.0, -1.797268},
            {0.0, 0.0, 1.0, 1.780801},
            {0.0, 0.0, 0.0, 1.0}
        };
        float    SDF_1_6_0__root_flat__1__cylinder__2_Height;
        float    SDF_1_6_0__root_flat__1__cylinder__2_Radius;
        float    SDF_1_6_0__root_flat__1__cylinder__2_Rounding;
        float4x4 SDF_1_6_1__root_flat__1__torus__1_SpaceTransform = {
            {1.0, 0.0, 0.0, -2.150593},
            {0.0, 1.0, 0.0, -2.560268},
            {0.0, 0.0, 1.0, 1.780801},
            {0.0, 0.0, 0.0, 1.0}
        };
        float    SDF_1_6_1__root_flat__1__torus__2_MainRadius;
        float    SDF_1_6_1__root_flat__1__torus__2_RingRadius;
        float2   SDF_1_6_1__root_flat__1__torus__2_Cap;
        float    SDF_1_7__root_two_cones__1_BlendFactor;
        float4x4 SDF_1_7_0__root_two_cones_cone__1_SpaceTransform = {
            {0.970696, -0.101283, 0.217924, -1.753172},
            {0.128052, 0.985376, -0.112415, -0.800878},
            {-0.203352, 0.137026, 0.96947, -4.215134},
            {0.0, 0.0, 0.0, 1.0}
        };
        float    SDF_1_7_0__root_two_cones_cone__2_Angle;
        float    SDF_1_7_0__root_two_cones_cone__2_Height;
        float4x4 SDF_1_7_1__root_two_cones_cone__1_SpaceTransform = {
            {-0.970696, 0.101283, -0.217924, 1.753172},
            {-0.128052, -0.985376, 0.112415, 1.516878},
            {-0.203352, 0.137026, 0.96947, -4.378134},
            {0.0, 0.0, 0.0, 1.0}
        };
        float SDF_1_7_1__root_two_cones_cone__2_Angle;
        float SDF_1_7_1__root_two_cones_cone__2_Height;
        ENDHLSL
        Pass
        {
            HLSLPROGRAM
            SdfResult SDF_1_0__root_ground__3(float3 p);
            SdfResult SDF_1_1_0__root_shroom_1_sphere_1__3(float3 p);
            SdfResult SDF_1_1_1__root_shroom_1_cone__3(float3 p);
            SdfResult SDF_1_2_0__root_shroom2_sphere_2__3(float3 p);
            SdfResult SDF_1_2_1__root_shroom2_cone_2__3(float3 p);
            SdfResult SDF_1_2_2__root_shroom2_sphere__3(float3 p);
            SdfResult SDF_1_3_0__root_combine_sphere__3(float3 p);
            SdfResult SDF_1_3_1__root_combine_sphere__3(float3 p);
            SdfResult SDF_1_3__root_combine__1(float3 p);
            SdfResult SDF_1_4_0__root_screw_cylinder__3(float3 p);
            SdfResult SDF_1_4_1_0__root_screw_head_head__3(float3 p);
            SdfResult SDF_1_4_1_1_0__root_screw_head_cuts_cut_1__3(float3 p);
            SdfResult SDF_1_4_1_1_1__root_screw_head_cuts_cut_2__3(float3 p);
            SdfResult SDF_1_4_1_1__root_screw_head_cuts__1(float3 p);
            SdfResult SDF_1_4_1__root_screw_head__1(float3 p);
            SdfResult SDF_1_5_0__root_q_cylinder__3(float3 p);
            SdfResult SDF_1_5_1__root_q_torus__3(float3 p);
            SdfResult SDF_1_5__root_q__1(float3 p);
            SdfResult SDF_1_6_0__root_flat__1__cylinder__3(float3 p);
            SdfResult SDF_1_6_1__root_flat__1__torus__3(float3 p);
            SdfResult SDF_1_6__root_flat__1___1(float3 p);
            SdfResult SDF_1_7_0__root_two_cones_cone__3(float3 p);
            SdfResult SDF_1_7_1__root_two_cones_cone__3(float3 p);
            SdfResult SDF_1_7__root_two_cones__1(float3 p);
            SdfResult SDF_1__root__1(float3 p);

            SdfResult SDF_1_0__root_ground__3(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = sdf::primitives3D::box(sdf::operators::transform(p, SDF_1_0__root_ground__1_SpaceTransform), SDF_1_0__root_ground__2_BoxExtents);
                result.id = int4(0, 0, 0, 4);
                return result;
            }

            SdfResult SDF_1_1_0__root_shroom_1_sphere_1__3(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = sdf::primitives3D::sphere(
                    sdf::operators::transform(p, SDF_1_1_0__root_shroom_1_sphere_1__1_SpaceTransform), SDF_1_1_0__root_shroom_1_sphere_1__2_Radius
                );
                result.id = int4(0, 0, 0, 7);
                return result;
            }

            SdfResult SDF_1_1_1__root_shroom_1_cone__3(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = sdf::primitives3D::cone(
                    (sdf::operators::transform(p, SDF_1_1_1__root_shroom_1_cone__1_SpaceTransform) - float3(0, SDF_1_1_1__root_shroom_1_cone__2_Height, 0)),
                    SDF_1_1_1__root_shroom_1_cone__2_Angle, SDF_1_1_1__root_shroom_1_cone__2_Height
                );
                result.id = int4(0, 0, 0, 10);
                return result;
            }

            SdfResult SDF_1_2_0__root_shroom2_sphere_2__3(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = sdf::primitives3D::sphere(
                    sdf::operators::transform(p, SDF_1_2_0__root_shroom2_sphere_2__1_SpaceTransform), SDF_1_2_0__root_shroom2_sphere_2__2_Radius
                );
                result.id = int4(0, 0, 0, 13);
                return result;
            }

            SdfResult SDF_1_2_1__root_shroom2_cone_2__3(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = sdf::primitives3D::cone(
                    (sdf::operators::transform(p, SDF_1_2_1__root_shroom2_cone_2__1_SpaceTransform) - float3(0, SDF_1_2_1__root_shroom2_cone_2__2_Height, 0)),
                    SDF_1_2_1__root_shroom2_cone_2__2_Angle, SDF_1_2_1__root_shroom2_cone_2__2_Height
                );
                result.id = int4(0, 0, 0, 16);
                return result;
            }

            SdfResult SDF_1_2_2__root_shroom2_sphere__3(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = sdf::primitives3D::sphere(
                    sdf::operators::transform(p, SDF_1_2_2__root_shroom2_sphere__1_SpaceTransform), SDF_1_2_2__root_shroom2_sphere__2_Radius
                );
                result.id = int4(0, 0, 0, 19);
                return result;
            }

            SdfResult SDF_1_3_0__root_combine_sphere__3(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = sdf::primitives3D::sphere(
                    sdf::operators::transform(p, SDF_1_3_0__root_combine_sphere__1_SpaceTransform), SDF_1_3_0__root_combine_sphere__2_Radius
                );
                result.id = int4(0, 0, 0, 23);
                return result;
            }

            SdfResult SDF_1_3_1__root_combine_sphere__3(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = sdf::primitives3D::sphere(
                    sdf::operators::transform(p, SDF_1_3_1__root_combine_sphere__1_SpaceTransform), SDF_1_3_1__root_combine_sphere__2_Radius
                );
                result.id = int4(0, 0, 0, 26);
                return result;
            }

            SdfResult SDF_1_3__root_combine__1(float3 p) {
                SdfResult result = SDF_1_3_0__root_combine_sphere__3(p);
                result = sdf::operators::intersectSimple(result, SDF_1_3_1__root_combine_sphere__3(p));
                result.id = int4(0, 0, 0, 20);
                return result;
            }

            SdfResult SDF_1_4_0__root_screw_cylinder__3(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = sdf::primitives3D::cylinder_capped(
                    sdf::operators::transform(p, SDF_1_4_0__root_screw_cylinder__1_SpaceTransform), SDF_1_4_0__root_screw_cylinder__2_Height,
                    SDF_1_4_0__root_screw_cylinder__2_Radius
                );
                result.id = int4(0, 0, 0, 29);
                return result;
            }

            SdfResult SDF_1_4_1_0__root_screw_head_head__3(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = sdf::primitives3D::sphere(
                    sdf::operators::transform(p, SDF_1_4_1_0__root_screw_head_head__1_SpaceTransform), SDF_1_4_1_0__root_screw_head_head__2_Radius
                );
                result.id = int4(0, 0, 0, 33);
                return result;
            }

            SdfResult SDF_1_4_1_1_0__root_screw_head_cuts_cut_1__3(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = sdf::primitives3D::box(
                    sdf::operators::transform(p, SDF_1_4_1_1_0__root_screw_head_cuts_cut_1__1_SpaceTransform), SDF_1_4_1_1_0__root_screw_head_cuts_cut_1__2_BoxExtents
                );
                result.id = int4(0, 0, 0, 37);
                return result;
            }

            SdfResult SDF_1_4_1_1_1__root_screw_head_cuts_cut_2__3(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = sdf::primitives3D::box(
                    sdf::operators::transform(p, SDF_1_4_1_1_1__root_screw_head_cuts_cut_2__1_SpaceTransform), SDF_1_4_1_1_1__root_screw_head_cuts_cut_2__2_BoxExtents
                );
                result.id = int4(0, 0, 0, 40);
                return result;
            }

            SdfResult SDF_1_4_1_1__root_screw_head_cuts__1(float3 p) {
                SdfResult result = SDF_1_4_1_1_0__root_screw_head_cuts_cut_1__3(p);
                result = sdf::operators::unionSimple(result, SDF_1_4_1_1_1__root_screw_head_cuts_cut_2__3(p));
                result.distance *= -1;
                return result;
            }

            SdfResult SDF_1_4_1__root_screw_head__1(float3 p) {
                SdfResult result = SDF_1_4_1_0__root_screw_head_head__3(p);
                result = sdf::operators::intersectSimple(result, SDF_1_4_1_1__root_screw_head_cuts__1(p));
                return result;
            }

            SdfResult SDF_1_5_0__root_q_cylinder__3(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = sdf::primitives3D::cylinder_rounded(
                    sdf::operators::transform(p, SDF_1_5_0__root_q_cylinder__1_SpaceTransform), SDF_1_5_0__root_q_cylinder__2_Radius, SDF_1_5_0__root_q_cylinder__2_Rounding,
                    SDF_1_5_0__root_q_cylinder__2_Height
                );
                result.id = int4(0, 0, 0, 44);
                return result;
            }

            SdfResult SDF_1_5_1__root_q_torus__3(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = -sdf::primitives3D::torus(
                    sdf::operators::transform(p, SDF_1_5_1__root_q_torus__1_SpaceTransform), SDF_1_5_1__root_q_torus__2_MainRadius, SDF_1_5_1__root_q_torus__2_RingRadius
                );
                result.id = int4(0, 0, 0, 47);
                return result;
            }

            SdfResult SDF_1_5__root_q__1(float3 p) {
                SdfResult result = SDF_1_5_0__root_q_cylinder__3(p);
                result = sdf::operators::intersectSimple(result, SDF_1_5_1__root_q_torus__3(p));
                return result;
            }

            SdfResult SDF_1_6_0__root_flat__1__cylinder__3(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = sdf::primitives3D::cylinder_infinite(
                    sdf::operators::transform(p, SDF_1_6_0__root_flat__1__cylinder__1_SpaceTransform), SDF_1_6_0__root_flat__1__cylinder__2_Radius
                );
                result.id = int4(0, 0, 0, 51);
                return result;
            }

            SdfResult SDF_1_6_1__root_flat__1__torus__3(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = -sdf::primitives3D::torus(
                    sdf::operators::transform(p, SDF_1_6_1__root_flat__1__torus__1_SpaceTransform), SDF_1_6_1__root_flat__1__torus__2_MainRadius,
                    SDF_1_6_1__root_flat__1__torus__2_RingRadius
                );
                result.id = int4(0, 0, 0, 54);
                return result;
            }

            SdfResult SDF_1_6__root_flat__1___1(float3 p) {
                SdfResult result = SDF_1_6_0__root_flat__1__cylinder__3(p);
                result = sdf::operators::intersectSimple(result, SDF_1_6_1__root_flat__1__torus__3(p));
                return result;
            }

            SdfResult SDF_1_7_0__root_two_cones_cone__3(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = sdf::primitives3D::cone(
                    (sdf::operators::transform(p, SDF_1_7_0__root_two_cones_cone__1_SpaceTransform) - float3(0, SDF_1_7_0__root_two_cones_cone__2_Height, 0)),
                    SDF_1_7_0__root_two_cones_cone__2_Angle, SDF_1_7_0__root_two_cones_cone__2_Height
                );
                result.id = int4(0, 0, 0, 58);
                return result;
            }

            SdfResult SDF_1_7_1__root_two_cones_cone__3(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = sdf::primitives3D::cone(
                    (sdf::operators::transform(p, SDF_1_7_1__root_two_cones_cone__1_SpaceTransform) - float3(0, SDF_1_7_1__root_two_cones_cone__2_Height, 0)),
                    SDF_1_7_1__root_two_cones_cone__2_Angle, SDF_1_7_1__root_two_cones_cone__2_Height
                );
                result.id = int4(0, 0, 0, 61);
                return result;
            }

            SdfResult SDF_1_7__root_two_cones__1(float3 p) {
                SdfResult result = SDF_1_7_0__root_two_cones_cone__3(p);
                result = sdf::operators::intersectSimple(result, SDF_1_7_1__root_two_cones_cone__3(p));
                return result;
            }

            SdfResult SDF_1__root__1(float3 p) {
                SdfResult result = SDF_1_0__root_ground__3(p);
                result = sdf::operators::unionSmooth(result, SDF_1_1_0__root_shroom_1_sphere_1__3(p), SDF_1__root__1_BlendFactor);
                result = sdf::operators::unionSmooth(result, SDF_1_1_1__root_shroom_1_cone__3(p), SDF_1__root__1_BlendFactor);
                result = sdf::operators::unionSmooth(result, SDF_1_2_0__root_shroom2_sphere_2__3(p), SDF_1__root__1_BlendFactor);
                result = sdf::operators::unionSmooth(result, SDF_1_2_1__root_shroom2_cone_2__3(p), SDF_1__root__1_BlendFactor);
                result = sdf::operators::unionSmooth(result, SDF_1_2_2__root_shroom2_sphere__3(p), SDF_1__root__1_BlendFactor);
                result = sdf::operators::unionSmooth(result, SDF_1_3__root_combine__1(p), SDF_1__root__1_BlendFactor);
                result = sdf::operators::unionSmooth(result, SDF_1_4_0__root_screw_cylinder__3(p), SDF_1__root__1_BlendFactor);
                result = sdf::operators::unionSmooth(result, SDF_1_4_1__root_screw_head__1(p), SDF_1__root__1_BlendFactor);
                result = sdf::operators::unionSmooth(result, SDF_1_5__root_q__1(p), SDF_1__root__1_BlendFactor);
                result = sdf::operators::unionSmooth(result, SDF_1_6__root_flat__1___1(p), SDF_1__root__1_BlendFactor);
                result = sdf::operators::unionSmooth(result, SDF_1_7__root_two_cones__1(p), SDF_1__root__1_BlendFactor);
                return result;
            }

            SdfResult sdfScene(float3 p) {
                return SDF_1__root__1(p);
            }
            ENDHLSL
        }
    }
}