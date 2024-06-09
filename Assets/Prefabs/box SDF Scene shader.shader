// GENERATED SHADER CONTENT. ANY MODIFICATIONS WILL BE OVERWRITTEN.
// Last modification: 09/06/2024 02:21:51

Shader "Box SDF Scene (generated)"
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
        [KeywordEnum (Material, Albedo, Skybox, Normal, Steps, Depth, Occlusion)]
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
        SDF_1__root__1_BlendFactor ("Blend factor", Float) = 0.58

        [Header (rootground)]
        SDF_1_0__root_ground__2_BoxExtents ("Box extents", Vector) = (6.442449, 0.169183, 3.634491, 0.0)

        [Header (rootshroom 1leg)]
        SDF_1_1_0__root_shroom_1_leg__2_Radius ("Sphere radius", Float) = 0.582681

        [Header (rootshroom 1cap)]
        SDF_1_1_1__root_shroom_1_cap__2_Angle ("Cone angle", Float) = 0.924753
        SDF_1_1_1__root_shroom_1_cap__2_Height ("Cone height", Float) = 0.663531

        [Header (rootshroom2leg)]
        SDF_1_2_0__root_shroom2_leg__2_Radius ("Sphere radius", Float) = 0.264981

        [Header (rootshroom2cone)]
        SDF_1_2_1__root_shroom2_cone__2_Angle ("Cone angle", Float) = 0.656545
        SDF_1_2_1__root_shroom2_cone__2_Height ("Cone height", Float) = 1.02

        [Header (rootshroom2cap)]
        SDF_1_2_2__root_shroom2_cap__2_Radius ("Sphere radius", Float) = 0.339796

        [Header (rootpill)]
        SDF_1_3__root_pill__1_BlendFactor ("Blend factor", Float) = 0.608677

        [Header (rootpillsphere)]
        SDF_1_3_0__root_pill_sphere__2_Radius ("Sphere radius", Float) = 0.538078

        [Header (rootpillsphere)]
        SDF_1_3_1__root_pill_sphere__2_Radius ("Sphere radius", Float) = 0.422096

        [Header (rootscrewbody)]
        SDF_1_4_0__root_screw_body__2_Height ("Cylinder height", Float) = 1.399991
        SDF_1_4_0__root_screw_body__2_Radius ("Cylinder radius", Float) = 0.404426
        SDF_1_4_0__root_screw_body__2_Rounding ("Rounding", Float) = 0.25

        [Header (rootscrewhead)]
        SDF_1_4_1__root_screw_head__1_BlendFactor ("Blend factor", Float) = 1.0

        [Header (rootscrewheadball)]
        SDF_1_4_1_0__root_screw_head_ball__2_Radius ("Sphere radius", Float) = 0.53636

        [Header (rootscrewheadcut 1)]
        SDF_1_4_1_1__root_screw_head_cut_1__2_BoxExtents ("Box extents", Vector) = (0.413049, 0.589912, 0.117837, 0.0)

        [Header (rootscrewheadcut 2)]
        SDF_1_4_1_2__root_screw_head_cut_2__2_BoxExtents ("Box extents", Vector) = (0.696923, 0.091037, 0.585179, 0.0)

        [Header (rootpeg)]
        SDF_1_5__root_peg__1_BlendFactor ("Blend factor", Float) = 1.0

        [Header (rootpegcylinder)]
        SDF_1_5_0__root_peg_cylinder__2_Height ("Cylinder height", Float) = 1.442987
        SDF_1_5_0__root_peg_cylinder__2_Radius ("Cylinder radius", Float) = 0.558908
        SDF_1_5_0__root_peg_cylinder__2_Rounding ("Rounding", Float) = 0.11

        [Header (rootpegcutout)]
        SDF_1_5_1__root_peg_cutout__2_MainRadius ("Torus main radius", Float) = 0.859511
        SDF_1_5_1__root_peg_cutout__2_RingRadius ("Torus ring radius", Float) = 0.461763
        SDF_1_5_1__root_peg_cutout__2_Cap ("Torus cap", Vector) = (0.53, -0.18, 0.0, 0.0)

        [Header (rootinfinitecylinder)]
        SDF_1_6__root_infinite_cylinder__1_BlendFactor ("Blend factor", Float) = 1.0

        [Header (rootinfinitecylindercylinder)]
        SDF_1_6_0__root_infinite_cylinder_cylinder__2_Height ("Cylinder height", Float) = 1.426067
        SDF_1_6_0__root_infinite_cylinder_cylinder__2_Radius ("Cylinder radius", Float) = 0.73139
        SDF_1_6_0__root_infinite_cylinder_cylinder__2_Rounding ("Rounding", Float) = 0.0

        [Header (rootinfinitecylindercutout)]
        SDF_1_6_1__root_infinite_cylinder_cutout__2_MainRadius ("Torus main radius", Float) = 0.859511
        SDF_1_6_1__root_infinite_cylinder_cutout__2_RingRadius ("Torus ring radius", Float) = 0.233685
        SDF_1_6_1__root_infinite_cylinder_cutout__2_Cap ("Torus cap", Vector) = (0.53, -0.18, 0.0, 0.0)

        [Header (roottwo cones)]
        SDF_1_7__root_two_cones__1_BlendFactor ("Blend factor", Float) = 0.46

        [Header (roottwo conescone)]
        SDF_1_7_0__root_two_cones_cone__2_Angle ("Cone angle", Float) = 0.46199
        SDF_1_7_0__root_two_cones_cone__2_Height ("Cone height", Float) = 1.258141

        [Header (roottwo conescone)]
        SDF_1_7_1__root_two_cones_cone__2_Angle ("Cone angle", Float) = 0.383333
        SDF_1_7_1__root_two_cones_cone__2_Height ("Cone height", Float) = 1.199648
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
        float4x4 SDF_1_0__root_ground__1_SpaceTransform = {
            {0.985253, 0.039483, 0.166489, 0.363901},
            {-0.049748, 0.99708, 0.057943, -0.130124},
            {-0.163715, -0.065371, 0.98434, 0.229378},
            {0.0, 0.0, 0.0, 1.0}
        };
        float3   SDF_1_0__root_ground__2_BoxExtents;
        float4x4 SDF_1_1_0__root_shroom_1_leg__1_SpaceTransform = {
            {0.970696, -0.101283, 0.217924, -0.883172},
            {0.128052, 0.985376, -0.112415, -0.454878},
            {-0.203352, 0.137026, 0.96947, -0.815134},
            {0.0, 0.0, 0.0, 1.0}
        };
        float    SDF_1_1_0__root_shroom_1_leg__2_Radius;
        float4x4 SDF_1_1_1__root_shroom_1_cap__1_SpaceTransform = {
            {0.939308, 0.069367, -0.335989, -0.389115},
            {-0.061828, 0.997538, 0.033097, -1.134318},
            {0.337457, -0.010315, 0.941284, -1.113102},
            {0.0, 0.0, 0.0, 1.0}
        };
        float    SDF_1_1_1__root_shroom_1_cap__2_Angle;
        float    SDF_1_1_1__root_shroom_1_cap__2_Height;
        float4x4 SDF_1_2_0__root_shroom2_leg__1_SpaceTransform = {
            {-0.02342, 0.103423, -0.994362, -1.404643},
            {0.097179, 0.99016, 0.100697, -0.47743},
            {0.994991, -0.094273, -0.03324, 1.135447},
            {0.0, 0.0, 0.0, 1.0}
        };
        float    SDF_1_2_0__root_shroom2_leg__2_Radius;
        float4x4 SDF_1_2_1__root_shroom2_cone__1_SpaceTransform = {
            {-0.773348, 0.262247, -0.5772, -1.871582},
            {0.241606, 0.963641, 0.114114, -0.697886},
            {0.586139, -0.051205, -0.808591, -0.364815},
            {0.0, 0.0, 0.0, 1.0}
        };
        float    SDF_1_2_1__root_shroom2_cone__2_Angle;
        float    SDF_1_2_1__root_shroom2_cone__2_Height;
        float4x4 SDF_1_2_2__root_shroom2_cap__1_SpaceTransform = {
            {-0.212623, 0.246355, -0.945569, -1.822549},
            {0.23447, 0.952287, 0.195382, -1.333403},
            {0.948586, -0.180165, -0.260241, 0.84638},
            {0.0, 0.0, 0.0, 1.0}
        };
        float    SDF_1_2_2__root_shroom2_cap__2_Radius;
        float    SDF_1_3__root_pill__1_BlendFactor;
        float4x4 SDF_1_3_0__root_pill_sphere__1_SpaceTransform = {
            {-0.763447, -0.052902, 0.088036, -1.502791},
            {0.064521, 0.266704, 0.719795, -0.023237},
            {-0.079912, 0.720743, -0.259892, -0.671471},
            {0.0, 0.0, 0.0, 1.0}
        };
        float    SDF_1_3_0__root_pill_sphere__2_Radius;
        float4x4 SDF_1_3_1__root_pill_sphere__1_SpaceTransform = {
            {-0.763447, -0.052902, 0.088036, -2.080791},
            {0.064521, 0.266704, 0.719795, -0.154237},
            {-0.079912, 0.720743, -0.259892, -0.671471},
            {0.0, 0.0, 0.0, 1.0}
        };
        float    SDF_1_3_1__root_pill_sphere__2_Radius;
        float4x4 SDF_1_4_0__root_screw_body__1_SpaceTransform = {
            {0.347078, -0.299924, 0.888585, -0.455998},
            {-0.301492, 0.861506, 0.408546, -2.532097},
            {-0.888054, -0.409698, 0.208585, -2.923648},
            {0.0, 0.0, 0.0, 1.0}
        };
        float    SDF_1_4_0__root_screw_body__2_Height;
        float    SDF_1_4_0__root_screw_body__2_Radius;
        float    SDF_1_4_0__root_screw_body__2_Rounding;
        float    SDF_1_4_1__root_screw_head__1_BlendFactor;
        float4x4 SDF_1_4_1_0__root_screw_head_ball__1_SpaceTransform = {
            {0.355245, -0.296137, 0.886625, -0.429045},
            {-0.301492, 0.861506, 0.408546, -4.122097},
            {-0.884818, -0.412444, 0.216763, -2.927725},
            {0.0, 0.0, 0.0, 1.0}
        };
        float    SDF_1_4_1_0__root_screw_head_ball__2_Radius;
        float4x4 SDF_1_4_1_1__root_screw_head_cut_1__1_SpaceTransform = {
            {0.301492, -0.861506, -0.408546, 4.421097},
            {0.355245, -0.296137, 0.886625, -0.429044},
            {-0.884819, -0.412444, 0.216762, -2.927724},
            {0.0, 0.0, 0.0, 1.0}
        };
        float3   SDF_1_4_1_1__root_screw_head_cut_1__2_BoxExtents;
        float4x4 SDF_1_4_1_2__root_screw_head_cut_2__1_SpaceTransform = {
            {0.301492, -0.861506, -0.408546, 4.550097},
            {0.355245, -0.296137, 0.886625, -0.429044},
            {-0.884819, -0.412444, 0.216762, -2.927725},
            {0.0, 0.0, 0.0, 1.0}
        };
        float3   SDF_1_4_1_2__root_screw_head_cut_2__2_BoxExtents;
        float    SDF_1_5__root_peg__1_BlendFactor;
        float4x4 SDF_1_5_0__root_peg_cylinder__1_SpaceTransform = {
            {1.0, 0.0, 0.0, -3.582294},
            {0.0, 1.0, 0.0, -1.580709},
            {0.0, 0.0, 1.0, -3.369244},
            {0.0, 0.0, 0.0, 1.0}
        };
        float    SDF_1_5_0__root_peg_cylinder__2_Height;
        float    SDF_1_5_0__root_peg_cylinder__2_Radius;
        float    SDF_1_5_0__root_peg_cylinder__2_Rounding;
        float4x4 SDF_1_5_1__root_peg_cutout__1_SpaceTransform = {
            {1.0, 0.0, 0.0, -3.582294},
            {0.0, 1.0, 0.0, -2.343709},
            {0.0, 0.0, 1.0, -3.369244},
            {0.0, 0.0, 0.0, 1.0}
        };
        float    SDF_1_5_1__root_peg_cutout__2_MainRadius;
        float    SDF_1_5_1__root_peg_cutout__2_RingRadius;
        float2   SDF_1_5_1__root_peg_cutout__2_Cap;
        float    SDF_1_6__root_infinite_cylinder__1_BlendFactor;
        float4x4 SDF_1_6_0__root_infinite_cylinder_cylinder__1_SpaceTransform = {
            {0.66883, 0.00523, -0.743398, -2.774013},
            {-0.024016, 0.999605, -0.014575, -1.770684},
            {0.743028, 0.027601, 0.668691, -0.469365},
            {0.0, 0.0, 0.0, 1.0}
        };
        float    SDF_1_6_0__root_infinite_cylinder_cylinder__2_Height;
        float    SDF_1_6_0__root_infinite_cylinder_cylinder__2_Radius;
        float    SDF_1_6_0__root_infinite_cylinder_cylinder__2_Rounding;
        float4x4 SDF_1_6_1__root_infinite_cylinder_cutout__1_SpaceTransform = {
            {0.66883, 0.00523, -0.743398, -2.774012},
            {-0.024016, 0.999605, -0.014575, -2.533684},
            {0.743028, 0.027601, 0.668691, -0.469365},
            {0.0, 0.0, 0.0, 1.0}
        };
        float    SDF_1_6_1__root_infinite_cylinder_cutout__2_MainRadius;
        float    SDF_1_6_1__root_infinite_cylinder_cutout__2_RingRadius;
        float2   SDF_1_6_1__root_infinite_cylinder_cutout__2_Cap;
        float    SDF_1_7__root_two_cones__1_BlendFactor;
        float4x4 SDF_1_7_0__root_two_cones_cone__1_SpaceTransform = {
            {0.970696, -0.101283, 0.217924, -3.285172},
            {0.128052, 0.985376, -0.112415, -1.387878},
            {-0.203352, 0.137026, 0.96947, 0.130866},
            {0.0, 0.0, 0.0, 1.0}
        };
        float    SDF_1_7_0__root_two_cones_cone__2_Angle;
        float    SDF_1_7_0__root_two_cones_cone__2_Height;
        float4x4 SDF_1_7_1__root_two_cones_cone__1_SpaceTransform = {
            {-0.637143, 0.056676, -0.76866, 2.78795},
            {-0.131973, -0.990587, 0.036353, 2.333473},
            {-0.759363, 0.124605, 0.638625, 1.93194},
            {0.0, 0.0, 0.0, 1.0}
        };
        float SDF_1_7_1__root_two_cones_cone__2_Angle;
        float SDF_1_7_1__root_two_cones_cone__2_Height;
        ENDHLSL
        Pass
        {
            HLSLPROGRAM
            SdfResult SDF_1_0__root_ground__3(float3 p);
            SdfResult SDF_1_1_0__root_shroom_1_leg__3(float3 p);
            SdfResult SDF_1_1_1__root_shroom_1_cap__3(float3 p);
            SdfResult SDF_1_2_0__root_shroom2_leg__3(float3 p);
            SdfResult SDF_1_2_1__root_shroom2_cone__3(float3 p);
            SdfResult SDF_1_2_2__root_shroom2_cap__3(float3 p);
            SdfResult SDF_1_3_0__root_pill_sphere__3(float3 p);
            SdfResult SDF_1_3_1__root_pill_sphere__3(float3 p);
            SdfResult SDF_1_3__root_pill__1(float3 p);
            SdfResult SDF_1_4_0__root_screw_body__3(float3 p);
            SdfResult SDF_1_4_1_0__root_screw_head_ball__3(float3 p);
            SdfResult SDF_1_4_1_1__root_screw_head_cut_1__3(float3 p);
            SdfResult SDF_1_4_1_2__root_screw_head_cut_2__3(float3 p);
            SdfResult SDF_1_4_1__root_screw_head__1(float3 p);
            SdfResult SDF_1_5_0__root_peg_cylinder__3(float3 p);
            SdfResult SDF_1_5_1__root_peg_cutout__3(float3 p);
            SdfResult SDF_1_5__root_peg__1(float3 p);
            SdfResult SDF_1_6_0__root_infinite_cylinder_cylinder__3(float3 p);
            SdfResult SDF_1_6_1__root_infinite_cylinder_cutout__3(float3 p);
            SdfResult SDF_1_6__root_infinite_cylinder__1(float3 p);
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

            SdfResult SDF_1_1_0__root_shroom_1_leg__3(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = sdf::primitives3D::sphere(sdf::operators::transform(p, SDF_1_1_0__root_shroom_1_leg__1_SpaceTransform), SDF_1_1_0__root_shroom_1_leg__2_Radius);
                result.id = int4(0, 0, 0, 7);
                return result;
            }

            SdfResult SDF_1_1_1__root_shroom_1_cap__3(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = sdf::primitives3D::cone(
                    (sdf::operators::transform(p, SDF_1_1_1__root_shroom_1_cap__1_SpaceTransform) - float3(0, SDF_1_1_1__root_shroom_1_cap__2_Height, 0)),
                    SDF_1_1_1__root_shroom_1_cap__2_Angle, SDF_1_1_1__root_shroom_1_cap__2_Height
                );
                result.id = int4(0, 0, 0, 10);
                return result;
            }

            SdfResult SDF_1_2_0__root_shroom2_leg__3(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = sdf::primitives3D::sphere(sdf::operators::transform(p, SDF_1_2_0__root_shroom2_leg__1_SpaceTransform), SDF_1_2_0__root_shroom2_leg__2_Radius);
                result.id = int4(0, 0, 0, 13);
                return result;
            }

            SdfResult SDF_1_2_1__root_shroom2_cone__3(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = sdf::primitives3D::cone(
                    (sdf::operators::transform(p, SDF_1_2_1__root_shroom2_cone__1_SpaceTransform) - float3(0, SDF_1_2_1__root_shroom2_cone__2_Height, 0)),
                    SDF_1_2_1__root_shroom2_cone__2_Angle, SDF_1_2_1__root_shroom2_cone__2_Height
                );
                result.id = int4(0, 0, 0, 16);
                return result;
            }

            SdfResult SDF_1_2_2__root_shroom2_cap__3(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = sdf::primitives3D::sphere(sdf::operators::transform(p, SDF_1_2_2__root_shroom2_cap__1_SpaceTransform), SDF_1_2_2__root_shroom2_cap__2_Radius);
                result.id = int4(0, 0, 0, 19);
                return result;
            }

            SdfResult SDF_1_3_0__root_pill_sphere__3(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = sdf::primitives3D::sphere(sdf::operators::transform(p, SDF_1_3_0__root_pill_sphere__1_SpaceTransform), SDF_1_3_0__root_pill_sphere__2_Radius);
                result.id = int4(0, 0, 0, 23);
                return result;
            }

            SdfResult SDF_1_3_1__root_pill_sphere__3(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = sdf::primitives3D::sphere(sdf::operators::transform(p, SDF_1_3_1__root_pill_sphere__1_SpaceTransform), SDF_1_3_1__root_pill_sphere__2_Radius);
                result.id = int4(0, 0, 0, 26);
                return result;
            }

            SdfResult SDF_1_3__root_pill__1(float3 p) {
                SdfResult result = SDF_1_3_0__root_pill_sphere__3(p);
                result = sdf::operators::intersectSimple(result, SDF_1_3_1__root_pill_sphere__3(p));
                result.id = int4(0, 0, 0, 20);
                return result;
            }

            SdfResult SDF_1_4_0__root_screw_body__3(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = sdf::primitives3D::cylinder_capped(
                    sdf::operators::transform(p, SDF_1_4_0__root_screw_body__1_SpaceTransform), SDF_1_4_0__root_screw_body__2_Height, SDF_1_4_0__root_screw_body__2_Radius
                );
                result.id = int4(0, 0, 0, 29);
                return result;
            }

            SdfResult SDF_1_4_1_0__root_screw_head_ball__3(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = sdf::primitives3D::sphere(
                    sdf::operators::transform(p, SDF_1_4_1_0__root_screw_head_ball__1_SpaceTransform), SDF_1_4_1_0__root_screw_head_ball__2_Radius
                );
                result.id = int4(0, 0, 0, 33);
                return result;
            }

            SdfResult SDF_1_4_1_1__root_screw_head_cut_1__3(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = -sdf::primitives3D::box(
                    sdf::operators::transform(p, SDF_1_4_1_1__root_screw_head_cut_1__1_SpaceTransform), SDF_1_4_1_1__root_screw_head_cut_1__2_BoxExtents
                );
                result.id = int4(0, 0, 0, 36);
                return result;
            }

            SdfResult SDF_1_4_1_2__root_screw_head_cut_2__3(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = -sdf::primitives3D::box(
                    sdf::operators::transform(p, SDF_1_4_1_2__root_screw_head_cut_2__1_SpaceTransform), SDF_1_4_1_2__root_screw_head_cut_2__2_BoxExtents
                );
                result.id = int4(0, 0, 0, 39);
                return result;
            }

            SdfResult SDF_1_4_1__root_screw_head__1(float3 p) {
                SdfResult result = SDF_1_4_1_0__root_screw_head_ball__3(p);
                result = sdf::operators::intersectSimple(result, SDF_1_4_1_1__root_screw_head_cut_1__3(p));
                result = sdf::operators::intersectSimple(result, SDF_1_4_1_2__root_screw_head_cut_2__3(p));
                return result;
            }

            SdfResult SDF_1_5_0__root_peg_cylinder__3(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = sdf::primitives3D::cylinder_rounded(
                    sdf::operators::transform(p, SDF_1_5_0__root_peg_cylinder__1_SpaceTransform), SDF_1_5_0__root_peg_cylinder__2_Radius, SDF_1_5_0__root_peg_cylinder__2_Rounding,
                    SDF_1_5_0__root_peg_cylinder__2_Height
                );
                result.id = int4(0, 0, 0, 43);
                return result;
            }

            SdfResult SDF_1_5_1__root_peg_cutout__3(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = -sdf::primitives3D::torus(
                    sdf::operators::transform(p, SDF_1_5_1__root_peg_cutout__1_SpaceTransform), SDF_1_5_1__root_peg_cutout__2_MainRadius, SDF_1_5_1__root_peg_cutout__2_RingRadius
                );
                result.id = int4(0, 0, 0, 46);
                return result;
            }

            SdfResult SDF_1_5__root_peg__1(float3 p) {
                SdfResult result = SDF_1_5_0__root_peg_cylinder__3(p);
                result = sdf::operators::intersectSimple(result, SDF_1_5_1__root_peg_cutout__3(p));
                return result;
            }

            SdfResult SDF_1_6_0__root_infinite_cylinder_cylinder__3(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = sdf::primitives3D::cylinder_infinite(
                    sdf::operators::transform(p, SDF_1_6_0__root_infinite_cylinder_cylinder__1_SpaceTransform), SDF_1_6_0__root_infinite_cylinder_cylinder__2_Radius
                );
                result.id = int4(0, 0, 0, 50);
                return result;
            }

            SdfResult SDF_1_6_1__root_infinite_cylinder_cutout__3(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = -sdf::primitives3D::torus(
                    sdf::operators::transform(p, SDF_1_6_1__root_infinite_cylinder_cutout__1_SpaceTransform), SDF_1_6_1__root_infinite_cylinder_cutout__2_MainRadius,
                    SDF_1_6_1__root_infinite_cylinder_cutout__2_RingRadius
                );
                result.id = int4(0, 0, 0, 53);
                return result;
            }

            SdfResult SDF_1_6__root_infinite_cylinder__1(float3 p) {
                SdfResult result = SDF_1_6_0__root_infinite_cylinder_cylinder__3(p);
                result = sdf::operators::intersectSimple(result, SDF_1_6_1__root_infinite_cylinder_cutout__3(p));
                return result;
            }

            SdfResult SDF_1_7_0__root_two_cones_cone__3(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = sdf::primitives3D::cone(
                    (sdf::operators::transform(p, SDF_1_7_0__root_two_cones_cone__1_SpaceTransform) - float3(0, SDF_1_7_0__root_two_cones_cone__2_Height, 0)),
                    SDF_1_7_0__root_two_cones_cone__2_Angle, SDF_1_7_0__root_two_cones_cone__2_Height
                );
                result.id = int4(0, 0, 0, 57);
                return result;
            }

            SdfResult SDF_1_7_1__root_two_cones_cone__3(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = -sdf::primitives3D::cone(
                    (sdf::operators::transform(p, SDF_1_7_1__root_two_cones_cone__1_SpaceTransform) - float3(0, SDF_1_7_1__root_two_cones_cone__2_Height, 0)),
                    SDF_1_7_1__root_two_cones_cone__2_Angle, SDF_1_7_1__root_two_cones_cone__2_Height
                );
                result.id = int4(0, 0, 0, 60);
                return result;
            }

            SdfResult SDF_1_7__root_two_cones__1(float3 p) {
                SdfResult result = SDF_1_7_0__root_two_cones_cone__3(p);
                result = sdf::operators::intersectSimple(result, SDF_1_7_1__root_two_cones_cone__3(p));
                return result;
            }

            SdfResult SDF_1__root__1(float3 p) {
                SdfResult result = SDF_1_0__root_ground__3(p);
                result = sdf::operators::unionSmooth(result, SDF_1_1_0__root_shroom_1_leg__3(p), SDF_1__root__1_BlendFactor);
                result = sdf::operators::unionSmooth(result, SDF_1_1_1__root_shroom_1_cap__3(p), SDF_1__root__1_BlendFactor);
                result = sdf::operators::unionSmooth(result, SDF_1_2_0__root_shroom2_leg__3(p), SDF_1__root__1_BlendFactor);
                result = sdf::operators::unionSmooth(result, SDF_1_2_1__root_shroom2_cone__3(p), SDF_1__root__1_BlendFactor);
                result = sdf::operators::unionSmooth(result, SDF_1_2_2__root_shroom2_cap__3(p), SDF_1__root__1_BlendFactor);
                result = sdf::operators::unionSmooth(result, SDF_1_3__root_pill__1(p), SDF_1__root__1_BlendFactor);
                result = sdf::operators::unionSmooth(result, SDF_1_4_0__root_screw_body__3(p), SDF_1__root__1_BlendFactor);
                result = sdf::operators::unionSmooth(result, SDF_1_4_1__root_screw_head__1(p), SDF_1__root__1_BlendFactor);
                result = sdf::operators::unionSmooth(result, SDF_1_5__root_peg__1(p), SDF_1__root__1_BlendFactor);
                result = sdf::operators::unionSmooth(result, SDF_1_6__root_infinite_cylinder__1(p), SDF_1__root__1_BlendFactor);
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