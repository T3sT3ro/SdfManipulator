// GENERATED SHADER CONTENT. ANY MODIFICATIONS WILL BE OVERWRITTEN.
// Last modification: 01/05/2024 13:42:40

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
        _1_root_BlendFactor ("Blend factor", Float) = 0.58

        [Header (root_ground)]
        _1_0_root_ground_BoxExtents ("Box extents", Vector) = (6.442449, 0.169183, 3.634491, 0.0)

        [Header (root_shroom_1_sphere_1)]
        _1_1_0_root_shroom_1_sphere_1_Radius ("Sphere radius", Float) = 0.582681

        [Header (root_shroom_1_cone)]
        _1_1_1_root_shroom_1_cone_Angle ("Cone angle", Float) = 1.042445
        _1_1_1_root_shroom_1_cone_Height ("Cone height", Float) = 0.513682

        [Header (root_shroom2_sphere_2)]
        _1_2_0_root_shroom2_sphere_2_Radius ("Sphere radius", Float) = 0.264981

        [Header (root_shroom2_cone_2)]
        _1_2_1_root_shroom2_cone_2_Angle ("Cone angle", Float) = 0.656545
        _1_2_1_root_shroom2_cone_2_Height ("Cone height", Float) = 1.02

        [Header (root_shroom2_sphere)]
        _1_2_2_root_shroom2_sphere_Radius ("Sphere radius", Float) = 0.339796

        [Header (root_combine)]
        _1_3_root_combine_BlendFactor ("Blend factor", Float) = 0.608677

        [Header (root_combine_sphere)]
        _1_3_0_root_combine_sphere_Radius ("Sphere radius", Float) = 0.509075

        [Header (root_combine_sphere)]
        _1_3_1_root_combine_sphere_Radius ("Sphere radius", Float) = 0.422096

        [Header (root_screw_cylinder)]
        _1_4_0_root_screw_cylinder_Height ("Cylinder height", Float) = 1.399991
        _1_4_0_root_screw_cylinder_Radius ("Cylinder radius", Float) = 0.404426
        _1_4_0_root_screw_cylinder_Rounding ("Rounding", Float) = 0.25

        [Header (root_screw_head)]
        _1_4_1_root_screw_head_BlendFactor ("Blend factor", Float) = 1.0

        [Header (root_screw_head_head)]
        _1_4_1_0_root_screw_head_head_Radius ("Sphere radius", Float) = 0.53

        [Header (root_screw_head_cuts)]
        _1_4_1_1_root_screw_head_cuts_BlendFactor ("Blend factor", Float) = 1.0

        [Header (root_screw_head_cuts_cut_1)]
        _1_4_1_1_0_root_screw_head_cuts_cut_1_BoxExtents ("Box extents", Vector) = (0.413049, 0.589912, 0.117837, 0.0)

        [Header (root_screw_head_cuts_cut_2)]
        _1_4_1_1_1_root_screw_head_cuts_cut_2_BoxExtents ("Box extents", Vector) = (0.696923, 0.091037, 0.587924, 0.0)

        [Header (root_q)]
        _1_5_root_q_Length ("Elongation", Vector) = (1.96, 3.34, 1.24, 0.0)

        [Header (root_q)]
        _1_5_root_q_BlendFactor ("Blend factor", Float) = 1.0

        [Header (root_q_cylinder)]
        _1_5_0_root_q_cylinder_Height ("Cylinder height", Float) = 1.426067
        _1_5_0_root_q_cylinder_Radius ("Cylinder radius", Float) = 0.73139
        _1_5_0_root_q_cylinder_Rounding ("Rounding", Float) = 0.0

        [Header (root_q_torus)]
        _1_5_1_root_q_torus_MainRadius ("Torus main radius", Float) = 0.859511
        _1_5_1_root_q_torus_RingRadius ("Torus ring radius", Float) = 0.232351
        _1_5_1_root_q_torus_Cap ("Torus cap", Vector) = (0.53, -0.18, 0.0, 0.0)

        [Header (root_flat__1_)]
        _1_6_root_flat__1__BlendFactor ("Blend factor", Float) = 1.0

        [Header (root_flat__1__cylinder)]
        _1_6_0_root_flat__1__cylinder_Height ("Cylinder height", Float) = 1.426067
        _1_6_0_root_flat__1__cylinder_Radius ("Cylinder radius", Float) = 0.73139
        _1_6_0_root_flat__1__cylinder_Rounding ("Rounding", Float) = 0.0

        [Header (root_flat__1__torus)]
        _1_6_1_root_flat__1__torus_MainRadius ("Torus main radius", Float) = 0.859511
        _1_6_1_root_flat__1__torus_RingRadius ("Torus ring radius", Float) = 0.232351
        _1_6_1_root_flat__1__torus_Cap ("Torus cap", Vector) = (0.53, -0.18, 0.0, 0.0)

        [Header (root_combine)]
        _1_7_root_combine_BlendFactor ("Blend factor", Float) = 0.46

        [Header (root_combine_cone)]
        _1_7_0_root_combine_cone_Angle ("Cone angle", Float) = 0.46199
        _1_7_0_root_combine_cone_Height ("Cone height", Float) = 1.258141

        [Header (root_combine_cone)]
        _1_7_1_root_combine_cone_Angle ("Cone angle", Float) = 0.518797
        _1_7_1_root_combine_cone_Height ("Cone height", Float) = 1.751412
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
        Pass
        {
            HLSLPROGRAM
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

            float    _1_root_BlendFactor;
            float4x4 _1_root_SpaceTransform = {
                {0.970696, -0.101283, 0.217924, 0.0},
                {0.128052, 0.985376, -0.112415, 0.0},
                {-0.203352, 0.137026, 0.96947, 0.0},
                {0.0, 0.0, 0.0, 1.0}
            };
            float3   _1_0_root_ground_BoxExtents;
            float4x4 _1_0_root_ground_SpaceTransform = {
                {0.985253, 0.039483, 0.166489, 0.324491},
                {-0.049748, 0.99708, 0.057943, -0.128134},
                {-0.163715, -0.065371, 0.98434, 0.235927},
                {0.0, 0.0, 0.0, 1.0}
            };
            float    _1_1_0_root_shroom_1_sphere_1_Radius;
            float4x4 _1_1_0_root_shroom_1_sphere_1_SpaceTransform = {
                {0.970696, -0.101283, 0.217924, -1.052},
                {0.128052, 0.985376, -0.112415, -0.635},
                {-0.203352, 0.137026, 0.96947, 0.0},
                {0.0, 0.0, 0.0, 1.0}
            };
            float    _1_1_1_root_shroom_1_cone_Angle;
            float    _1_1_1_root_shroom_1_cone_Height;
            float4x4 _1_1_1_root_shroom_1_cone_SpaceTransform = {
                {0.939308, 0.069367, -0.335989, -0.983747},
                {-0.061828, 0.997538, 0.033097, -1.64916},
                {0.337457, -0.010315, 0.941284, -0.503346},
                {0.0, 0.0, 0.0, 1.0}
            };
            float    _1_2_0_root_shroom2_sphere_2_Radius;
            float4x4 _1_2_0_root_shroom2_sphere_2_SpaceTransform = {
                {-0.02342, 0.103423, -0.994362, -1.403706},
                {0.097179, 0.99016, 0.100697, -0.481317},
                {0.994991, -0.094273, -0.03324, 1.095647},
                {0.0, 0.0, 0.0, 1.0}
            };
            float    _1_2_1_root_shroom2_cone_2_Angle;
            float    _1_2_1_root_shroom2_cone_2_Height;
            float4x4 _1_2_1_root_shroom2_cone_2_SpaceTransform = {
                {-0.773348, 0.262247, -0.5772, -1.840792},
                {0.241606, 0.963641, 0.114114, -1.722459},
                {0.586139, -0.051205, -0.808591, -0.388022},
                {0.0, 0.0, 0.0, 1.0}
            };
            float    _1_2_2_root_shroom2_sphere_Radius;
            float4x4 _1_2_2_root_shroom2_sphere_SpaceTransform = {
                {-0.212623, 0.246355, -0.945569, -1.814044},
                {0.23447, 0.952287, 0.195382, -1.342781},
                {0.948586, -0.180165, -0.260241, 0.808436},
                {0.0, 0.0, 0.0, 1.0}
            };
            float    _1_3_root_combine_BlendFactor;
            float4x4 _1_3_root_combine_SpaceTransform = {
                {-0.034952, 0.834414, 0.550029, -0.580062},
                {0.810044, 0.345994, -0.47341, 1.560808},
                {-0.585327, 0.429001, -0.688005, -1.601026},
                {0.0, 0.0, 0.0, 1.0}
            };
            float    _1_3_0_root_combine_sphere_Radius;
            float4x4 _1_3_0_root_combine_sphere_SpaceTransform = {
                {-0.034952, 0.834414, 0.550029, -0.388063},
                {0.810044, 0.345994, -0.47341, 1.534808},
                {-0.585327, 0.429001, -0.688005, -1.601026},
                {0.0, 0.0, 0.0, 1.0}
            };
            float    _1_3_1_root_combine_sphere_Radius;
            float4x4 _1_3_1_root_combine_sphere_SpaceTransform = {
                {-0.034952, 0.834414, 0.550029, -0.966063},
                {0.810044, 0.345994, -0.47341, 1.403808},
                {-0.585327, 0.429001, -0.688005, -1.601026},
                {0.0, 0.0, 0.0, 1.0}
            };
            float    _1_4_0_root_screw_cylinder_Height;
            float    _1_4_0_root_screw_cylinder_Radius;
            float    _1_4_0_root_screw_cylinder_Rounding;
            float4x4 _1_4_0_root_screw_cylinder_SpaceTransform = {
                {0.347078, -0.299924, 0.888585, -0.469882},
                {-0.301492, 0.861506, 0.408546, -2.520037},
                {-0.888054, -0.409698, 0.208585, -2.888125},
                {0.0, 0.0, 0.0, 1.0}
            };
            float    _1_4_1_root_screw_head_BlendFactor;
            float4x4 _1_4_1_root_screw_head_SpaceTransform = {
                {0.355245, -0.296137, 0.886625, -0.443254},
                {-0.301492, 0.861506, 0.408546, -3.717037},
                {-0.884818, -0.412444, 0.216763, -2.892332},
                {0.0, 0.0, 0.0, 1.0}
            };
            float    _1_4_1_0_root_screw_head_head_Radius;
            float4x4 _1_4_1_0_root_screw_head_head_SpaceTransform = {
                {0.355245, -0.296137, 0.886625, -0.443254},
                {-0.301492, 0.861506, 0.408546, -4.110037},
                {-0.884818, -0.412444, 0.216763, -2.892332},
                {0.0, 0.0, 0.0, 1.0}
            };
            float    _1_4_1_1_root_screw_head_cuts_BlendFactor;
            float4x4 _1_4_1_1_root_screw_head_cuts_SpaceTransform = {
                {0.301492, -0.861506, -0.408546, 4.140037},
                {0.355245, -0.296137, 0.886625, -0.443254},
                {-0.884818, -0.412444, 0.216763, -2.892332},
                {0.0, 0.0, 0.0, 1.0}
            };
            float3   _1_4_1_1_0_root_screw_head_cuts_cut_1_BoxExtents;
            float4x4 _1_4_1_1_0_root_screw_head_cuts_cut_1_SpaceTransform = {
                {0.301492, -0.861506, -0.408546, 4.409037},
                {0.355245, -0.296137, 0.886625, -0.443254},
                {-0.884818, -0.412444, 0.216763, -2.892332},
                {0.0, 0.0, 0.0, 1.0}
            };
            float3   _1_4_1_1_1_root_screw_head_cuts_cut_2_BoxExtents;
            float4x4 _1_4_1_1_1_root_screw_head_cuts_cut_2_SpaceTransform = {
                {0.301492, -0.861506, -0.408546, 4.538037},
                {0.355245, -0.296137, 0.886625, -0.443254},
                {-0.884818, -0.412444, 0.216763, -2.892333},
                {0.0, 0.0, 0.0, 1.0}
            };
            float3   _1_5_root_q_Length;
            float    _1_5_root_q_BlendFactor;
            float4x4 _1_5_root_q_SpaceTransform = {
                {1.0, 0.0, 0.0, 2.898605},
                {0.0, 1.0, 0.0, -2.253964},
                {0.0, 0.0, 1.0, 1.780652},
                {0.0, 0.0, 0.0, 1.0}
            };
            float    _1_5_0_root_q_cylinder_Height;
            float    _1_5_0_root_q_cylinder_Radius;
            float    _1_5_0_root_q_cylinder_Rounding;
            float4x4 _1_5_0_root_q_cylinder_SpaceTransform = {
                {1.0, 0.0, 0.0, 2.898605},
                {0.0, 1.0, 0.0, -1.796963},
                {0.0, 0.0, 1.0, 1.780652},
                {0.0, 0.0, 0.0, 1.0}
            };
            float    _1_5_1_root_q_torus_MainRadius;
            float    _1_5_1_root_q_torus_RingRadius;
            float2   _1_5_1_root_q_torus_Cap;
            float4x4 _1_5_1_root_q_torus_SpaceTransform = {
                {1.0, 0.0, 0.0, 2.898605},
                {0.0, 1.0, 0.0, -2.559964},
                {0.0, 0.0, 1.0, 1.780652},
                {0.0, 0.0, 0.0, 1.0}
            };
            float    _1_6_root_flat__1__BlendFactor;
            float4x4 _1_6_root_flat__1__SpaceTransform = {
                {1.0, 0.0, 0.0, -2.190593},
                {0.0, 1.0, 0.0, -2.254268},
                {0.0, 0.0, 1.0, 1.780801},
                {0.0, 0.0, 0.0, 1.0}
            };
            float    _1_6_0_root_flat__1__cylinder_Height;
            float    _1_6_0_root_flat__1__cylinder_Radius;
            float    _1_6_0_root_flat__1__cylinder_Rounding;
            float4x4 _1_6_0_root_flat__1__cylinder_SpaceTransform = {
                {1.0, 0.0, 0.0, -2.190593},
                {0.0, 1.0, 0.0, -1.797268},
                {0.0, 0.0, 1.0, 1.780801},
                {0.0, 0.0, 0.0, 1.0}
            };
            float    _1_6_1_root_flat__1__torus_MainRadius;
            float    _1_6_1_root_flat__1__torus_RingRadius;
            float2   _1_6_1_root_flat__1__torus_Cap;
            float4x4 _1_6_1_root_flat__1__torus_SpaceTransform = {
                {1.0, 0.0, 0.0, -2.190593},
                {0.0, 1.0, 0.0, -2.560268},
                {0.0, 0.0, 1.0, 1.780801},
                {0.0, 0.0, 0.0, 1.0}
            };
            float    _1_7_root_combine_BlendFactor;
            float4x4 _1_7_root_combine_SpaceTransform = {
                {0.970696, -0.101283, 0.217924, -1.792},
                {0.128052, 0.985376, -0.112415, -0.962},
                {-0.203352, 0.137026, 0.96947, -4.369999},
                {0.0, 0.0, 0.0, 1.0}
            };
            float    _1_7_0_root_combine_cone_Angle;
            float    _1_7_0_root_combine_cone_Height;
            float4x4 _1_7_0_root_combine_cone_SpaceTransform = {
                {0.970696, -0.101283, 0.217924, -1.792},
                {0.128052, 0.985376, -0.112415, -2.064141},
                {-0.203352, 0.137026, 0.96947, -4.207},
                {0.0, 0.0, 0.0, 1.0}
            };
            float    _1_7_1_root_combine_cone_Angle;
            float    _1_7_1_root_combine_cone_Height;
            float4x4 _1_7_1_root_combine_cone_SpaceTransform = {
                {-0.970696, 0.101283, -0.217924, 1.792},
                {-0.128052, -0.985376, 0.112415, -0.229412},
                {-0.203352, 0.137026, 0.96947, -4.37},
                {0.0, 0.0, 0.0, 1.0}
            };

            SdfResult _1_0_root_ground(float3 p) {
                SdfResult result = (SdfResult)0;
                p = sdf::operators::transform(p, _1_0_root_ground_SpaceTransform);
                result.distance = sdf::primitives3D::box(p, _1_0_root_ground_BoxExtents);
                result.id = int4(0, 0, 0, 2);
                return result;
            }

            SdfResult _1_1_0_root_shroom_1_sphere_1(float3 p) {
                SdfResult result = (SdfResult)0;
                p = sdf::operators::transform(p, _1_1_0_root_shroom_1_sphere_1_SpaceTransform);
                result.distance = sdf::primitives3D::sphere(p, _1_1_0_root_shroom_1_sphere_1_Radius);
                result.id = int4(0, 0, 0, 3);
                return result;
            }

            SdfResult _1_1_1_root_shroom_1_cone(float3 p) {
                SdfResult result = (SdfResult)0;
                p = sdf::operators::transform(p, _1_1_1_root_shroom_1_cone_SpaceTransform);
                result.distance = sdf::primitives3D::cone(p, _1_1_1_root_shroom_1_cone_Angle, _1_1_1_root_shroom_1_cone_Height);
                result.id = int4(0, 0, 0, 4);
                return result;
            }

            SdfResult _1_2_0_root_shroom2_sphere_2(float3 p) {
                SdfResult result = (SdfResult)0;
                p = sdf::operators::transform(p, _1_2_0_root_shroom2_sphere_2_SpaceTransform);
                result.distance = sdf::primitives3D::sphere(p, _1_2_0_root_shroom2_sphere_2_Radius);
                result.id = int4(0, 0, 0, 5);
                return result;
            }

            SdfResult _1_2_1_root_shroom2_cone_2(float3 p) {
                SdfResult result = (SdfResult)0;
                p = sdf::operators::transform(p, _1_2_1_root_shroom2_cone_2_SpaceTransform);
                result.distance = sdf::primitives3D::cone(p, _1_2_1_root_shroom2_cone_2_Angle, _1_2_1_root_shroom2_cone_2_Height);
                result.id = int4(0, 0, 0, 6);
                return result;
            }

            SdfResult _1_2_2_root_shroom2_sphere(float3 p) {
                SdfResult result = (SdfResult)0;
                p = sdf::operators::transform(p, _1_2_2_root_shroom2_sphere_SpaceTransform);
                result.distance = sdf::primitives3D::sphere(p, _1_2_2_root_shroom2_sphere_Radius);
                result.id = int4(0, 0, 0, 7);
                return result;
            }

            SdfResult _1_3_0_root_combine_sphere(float3 p) {
                SdfResult result = (SdfResult)0;
                p = sdf::operators::transform(p, _1_3_0_root_combine_sphere_SpaceTransform);
                result.distance = sdf::primitives3D::sphere(p, _1_3_0_root_combine_sphere_Radius);
                result.id = int4(0, 0, 0, 9);
                return result;
            }

            SdfResult _1_3_1_root_combine_sphere(float3 p) {
                SdfResult result = (SdfResult)0;
                p = sdf::operators::transform(p, _1_3_1_root_combine_sphere_SpaceTransform);
                result.distance = -sdf::primitives3D::sphere(p, _1_3_1_root_combine_sphere_Radius);
                result.id = int4(0, 0, 0, 10);
                return result;
            }

            SdfResult _1_3_root_combine(float3 p) {
                SdfResult result = sdf::operators::intersectSimple(_1_3_0_root_combine_sphere(p), _1_3_1_root_combine_sphere(p));
                result.id = int4(0, 0, 0, 8);
                return result;
            }

            SdfResult _1_4_0_root_screw_cylinder(float3 p) {
                SdfResult result = (SdfResult)0;
                p = sdf::operators::transform(p, _1_4_0_root_screw_cylinder_SpaceTransform);
                result.distance = sdf::primitives3D::cylinder_capped(p, _1_4_0_root_screw_cylinder_Height, _1_4_0_root_screw_cylinder_Radius);
                result.id = int4(0, 0, 0, 11);
                return result;
            }

            SdfResult _1_4_1_0_root_screw_head_head(float3 p) {
                SdfResult result = (SdfResult)0;
                p = sdf::operators::transform(p, _1_4_1_0_root_screw_head_head_SpaceTransform);
                result.distance = sdf::primitives3D::sphere(p, _1_4_1_0_root_screw_head_head_Radius);
                result.id = int4(0, 0, 0, 13);
                return result;
            }

            SdfResult _1_4_1_1_0_root_screw_head_cuts_cut_1(float3 p) {
                SdfResult result = (SdfResult)0;
                p = sdf::operators::transform(p, _1_4_1_1_0_root_screw_head_cuts_cut_1_SpaceTransform);
                result.distance = sdf::primitives3D::box(p, _1_4_1_1_0_root_screw_head_cuts_cut_1_BoxExtents);
                result.id = int4(0, 0, 0, 15);
                return result;
            }

            SdfResult _1_4_1_1_1_root_screw_head_cuts_cut_2(float3 p) {
                SdfResult result = (SdfResult)0;
                p = sdf::operators::transform(p, _1_4_1_1_1_root_screw_head_cuts_cut_2_SpaceTransform);
                result.distance = sdf::primitives3D::box(p, _1_4_1_1_1_root_screw_head_cuts_cut_2_BoxExtents);
                result.id = int4(0, 0, 0, 16);
                return result;
            }

            SdfResult _1_4_1_1_root_screw_head_cuts(float3 p) {
                SdfResult result = sdf::operators::unionSimple(_1_4_1_1_0_root_screw_head_cuts_cut_1(p), _1_4_1_1_1_root_screw_head_cuts_cut_2(p));
                result.distance = -result.distance;
                return result;
            }

            SdfResult _1_4_1_root_screw_head(float3 p) {
                SdfResult result = sdf::operators::intersectSimple(_1_4_1_0_root_screw_head_head(p), _1_4_1_1_root_screw_head_cuts(p));
                return result;
            }

            SdfResult _1_5_0_root_q_cylinder(float3 p) {
                SdfResult result = (SdfResult)0;
                p = sdf::operators::transform(p, _1_5_0_root_q_cylinder_SpaceTransform);
                result.distance = sdf::primitives3D::cylinder_capped(p, _1_5_0_root_q_cylinder_Height, _1_5_0_root_q_cylinder_Radius);
                result.id = int4(0, 0, 0, 19);
                return result;
            }

            SdfResult _1_5_1_root_q_torus(float3 p) {
                SdfResult result = (SdfResult)0;
                p = sdf::operators::transform(p, _1_5_1_root_q_torus_SpaceTransform);
                result.distance = -sdf::primitives3D::torus(p, _1_5_1_root_q_torus_MainRadius, _1_5_1_root_q_torus_RingRadius);
                result.id = int4(0, 0, 0, 20);
                return result;
            }

            SdfResult _1_5_root_q(float3 p) {
                SdfResult result = sdf::operators::intersectSimple(_1_5_0_root_q_cylinder(p), _1_5_1_root_q_torus(p));
                return result;
            }

            SdfResult _1_6_0_root_flat__1__cylinder(float3 p) {
                SdfResult result = (SdfResult)0;
                p = sdf::operators::transform(p, _1_6_0_root_flat__1__cylinder_SpaceTransform);
                result.distance = sdf::primitives3D::cylinder_infinite(p, _1_6_0_root_flat__1__cylinder_Radius);
                result.id = int4(0, 0, 0, 22);
                return result;
            }

            SdfResult _1_6_1_root_flat__1__torus(float3 p) {
                SdfResult result = (SdfResult)0;
                p = sdf::operators::transform(p, _1_6_1_root_flat__1__torus_SpaceTransform);
                result.distance = -sdf::primitives3D::torus(p, _1_6_1_root_flat__1__torus_MainRadius, _1_6_1_root_flat__1__torus_RingRadius);
                result.id = int4(0, 0, 0, 23);
                return result;
            }

            SdfResult _1_6_root_flat__1_(float3 p) {
                SdfResult result = sdf::operators::intersectSimple(_1_6_0_root_flat__1__cylinder(p), _1_6_1_root_flat__1__torus(p));
                return result;
            }

            SdfResult _1_7_0_root_combine_cone(float3 p) {
                SdfResult result = (SdfResult)0;
                p = sdf::operators::transform(p, _1_7_0_root_combine_cone_SpaceTransform);
                result.distance = sdf::primitives3D::cone(p, _1_7_0_root_combine_cone_Angle, _1_7_0_root_combine_cone_Height);
                result.id = int4(0, 0, 0, 25);
                return result;
            }

            SdfResult _1_7_1_root_combine_cone(float3 p) {
                SdfResult result = (SdfResult)0;
                p = sdf::operators::transform(p, _1_7_1_root_combine_cone_SpaceTransform);
                result.distance = sdf::primitives3D::cone(p, _1_7_1_root_combine_cone_Angle, _1_7_1_root_combine_cone_Height);
                result.id = int4(0, 0, 0, 26);
                return result;
            }

            SdfResult _1_7_root_combine(float3 p) {
                SdfResult result = sdf::operators::intersectSimple(_1_7_0_root_combine_cone(p), _1_7_1_root_combine_cone(p));
                return result;
            }

            SdfResult _1_root(float3 p) {
                SdfResult result = sdf::operators::unionSmooth(
                    sdf::operators::unionSmooth(
                        sdf::operators::unionSmooth(
                            _1_0_root_ground(p), sdf::operators::unionSmooth(_1_1_0_root_shroom_1_sphere_1(p), _1_1_1_root_shroom_1_cone(p), _1_root_BlendFactor),
                            _1_root_BlendFactor
                        ), sdf::operators::unionSmooth(
                            _1_2_0_root_shroom2_sphere_2(p), sdf::operators::unionSmooth(_1_2_1_root_shroom2_cone_2(p), _1_2_2_root_shroom2_sphere(p), _1_root_BlendFactor),
                            _1_root_BlendFactor
                        ), _1_root_BlendFactor
                    ), sdf::operators::unionSmooth(
                        sdf::operators::unionSmooth(
                            _1_3_root_combine(p), sdf::operators::unionSmooth(_1_4_0_root_screw_cylinder(p), _1_4_1_root_screw_head(p), _1_root_BlendFactor), _1_root_BlendFactor
                        ), sdf::operators::unionSmooth(
                            _1_5_root_q(p), sdf::operators::unionSmooth(_1_6_root_flat__1_(p), _1_7_root_combine(p), _1_root_BlendFactor), _1_root_BlendFactor
                        ), _1_root_BlendFactor
                    ), _1_root_BlendFactor
                );
                return result;
            }

            SdfResult sdfScene(float3 p) {
                return _1_root(p);
            }
            ENDHLSL
        }
    }
}