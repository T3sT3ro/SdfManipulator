// GENERATED SHADER CONTENT. ANY MODIFICATIONS WILL BE OVERWRITTEN.
// Last modification: 18/06/2024 23:11:54

Shader "bunny (generated)"
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
        SDF_1__root__1_BlendFactor ("Blend factor", Float) = 0.06

        [Header (root head root ears)]
        SDF_1_0_0__root_head_root_ears__1_BlendFactor ("Blend factor", Float) = 0.73

        [Header (root head root ears q)]
        SDF_1_0_0_0__root_head_root_ears_q__2_Height ("Cylinder height", Float) = 0.680959
        SDF_1_0_0_0__root_head_root_ears_q__2_Radius ("Cylinder radius", Float) = 0.116658
        SDF_1_0_0_0__root_head_root_ears_q__2_Rounding ("Rounding", Float) = 0.07

        [Header (root head root ears q)]
        SDF_1_0_0_0__root_head_root_ears_q__3_Length ("Elongation", Vector) = (0.02, 0.0, 0.0, 0.0)

        [Header (root head root ears q)]
        SDF_1_0_0_1__root_head_root_ears_q__2_Height ("Cylinder height", Float) = 0.509504
        SDF_1_0_0_1__root_head_root_ears_q__2_Radius ("Cylinder radius", Float) = 0.078961
        SDF_1_0_0_1__root_head_root_ears_q__2_Rounding ("Rounding", Float) = -0.17

        [Header (root head root ears q)]
        SDF_1_0_0_1__root_head_root_ears_q__3_Length ("Elongation", Vector) = (0.08, 0.07, 0.0, 0.0)

        [Header (root head root ears ear base L)]
        SDF_1_0_0_2__root_head_root_ears_ear_base_L__2_Radius ("Sphere radius", Float) = 0.081883

        [Header (root head root ears ear base R)]
        SDF_1_0_0_3__root_head_root_ears_ear_base_R__2_Radius ("Sphere radius", Float) = 0.048229

        [Header (root head root head)]
        SDF_1_0_1__root_head_root_head__1_BlendFactor ("Blend factor", Float) = 0.19

        [Header (root head root head q)]
        SDF_1_0_1_0__root_head_root_head_q__2_Radius ("Sphere radius", Float) = 0.65

        [Header (root head root head q)]
        SDF_1_0_1_0__root_head_root_head_q__3_Length ("Elongation", Vector) = (0.12, 0.0, 0.0, 0.0)

        [Header (root head root head eyeL)]
        SDF_1_0_1_1__root_head_root_head_eyeL__2_MainRadius ("Torus main radius", Float) = 0.185469
        SDF_1_0_1_1__root_head_root_head_eyeL__2_RingRadius ("Torus ring radius", Float) = 0.04714
        SDF_1_0_1_1__root_head_root_head_eyeL__2_Cap ("Torus cap", Float) = 1.208449

        [Header (root head root head eyeR)]
        SDF_1_0_1_2__root_head_root_head_eyeR__2_MainRadius ("Torus main radius", Float) = 0.133103
        SDF_1_0_1_2__root_head_root_head_eyeR__2_RingRadius ("Torus ring radius", Float) = 0.042229
        SDF_1_0_1_2__root_head_root_head_eyeR__2_Cap ("Torus cap", Float) = 1.150463

        [Header (root head root head mouth)]
        SDF_1_0_1_3__root_head_root_head_mouth__2_MainRadius ("Torus main radius", Float) = 0.114369
        SDF_1_0_1_3__root_head_root_head_mouth__2_RingRadius ("Torus ring radius", Float) = 0.04861
        SDF_1_0_1_3__root_head_root_head_mouth__2_Cap ("Torus cap", Float) = 1.762102

        [Header (root torso)]
        SDF_1_1__root_torso__1_BlendFactor ("Blend factor", Float) = 1.0

        [Header (root torso cylinder)]
        SDF_1_1_0__root_torso_cylinder__2_Height ("Cylinder height", Float) = 1.0
        SDF_1_1_0__root_torso_cylinder__2_Radius ("Cylinder radius", Float) = 0.25
        SDF_1_1_0__root_torso_cylinder__2_Rounding ("Rounding", Float) = 0.25

        [Header (root torso belly)]
        SDF_1_1_1__root_torso_belly__2_Radius ("Sphere radius", Float) = 0.8229

        [Header (root torso torus)]
        SDF_1_1_2__root_torso_torus__2_MainRadius ("Torus main radius", Float) = 1.0
        SDF_1_1_2__root_torso_torus__2_RingRadius ("Torus ring radius", Float) = 0.096017
        SDF_1_1_2__root_torso_torus__2_Cap ("Torus cap", Float) = 0.640446

        [Header (root torso torus  2 )]
        SDF_1_1_3__root_torso_torus__2___2_MainRadius ("Torus main radius", Float) = 1.533898
        SDF_1_1_3__root_torso_torus__2___2_RingRadius ("Torus ring radius", Float) = 0.096017
        SDF_1_1_3__root_torso_torus__2___2_Cap ("Torus cap", Float) = 0.768122

        [Header (root torso torus  1 )]
        SDF_1_1_4__root_torso_torus__1___2_MainRadius ("Torus main radius", Float) = 1.0
        SDF_1_1_4__root_torso_torus__1___2_RingRadius ("Torus ring radius", Float) = 0.096017
        SDF_1_1_4__root_torso_torus__1___2_Cap ("Torus cap", Float) = 0.640446

        [Header (root torso torus  3 )]
        SDF_1_1_5__root_torso_torus__3___2_MainRadius ("Torus main radius", Float) = 1.0
        SDF_1_1_5__root_torso_torus__3___2_RingRadius ("Torus ring radius", Float) = 0.162079
        SDF_1_1_5__root_torso_torus__3___2_Cap ("Torus cap", Float) = 0.838926
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
        float    SDF_1_0_0__root_head_root_ears__1_BlendFactor;
        float4x4 SDF_1_0_0_0__root_head_root_ears_q__1_SpaceTransform = {
            {0.876204, 0.019977, 0.481527, 0.340481},
            {-0.327837, 0.757063, 0.565136, -1.505296},
            {-0.353256, -0.653036, 0.669891, 1.681305},
            {0.0, 0.0, 0.0, 1.0}
        };
        float    SDF_1_0_0_0__root_head_root_ears_q__2_Height;
        float    SDF_1_0_0_0__root_head_root_ears_q__2_Radius;
        float    SDF_1_0_0_0__root_head_root_ears_q__2_Rounding;
        float3   SDF_1_0_0_0__root_head_root_ears_q__3_Length;
        float4x4 SDF_1_0_0_1__root_head_root_ears_q__1_SpaceTransform = {
            {0.813388, -0.503336, 0.291639, 0.383809},
            {0.205647, 0.717763, 0.665225, -1.381917},
            {-0.54416, -0.481112, 0.68733, 1.390806},
            {0.0, 0.0, 0.0, 1.0}
        };
        float    SDF_1_0_0_1__root_head_root_ears_q__2_Height;
        float    SDF_1_0_0_1__root_head_root_ears_q__2_Radius;
        float    SDF_1_0_0_1__root_head_root_ears_q__2_Rounding;
        float3   SDF_1_0_0_1__root_head_root_ears_q__3_Length;
        float4x4 SDF_1_0_0_2__root_head_root_ears_ear_base_L__1_SpaceTransform = {
            {0.940201, -0.101083, 0.325277, 0.721522},
            {0.109353, 0.993977, -0.007193, -2.771248},
            {-0.322591, 0.042333, 0.945592, 0.115935},
            {0.0, 0.0, 0.0, 1.0}
        };
        float    SDF_1_0_0_2__root_head_root_ears_ear_base_L__2_Radius;
        float4x4 SDF_1_0_0_3__root_head_root_ears_ear_base_R__1_SpaceTransform = {
            {0.940201, -0.101083, 0.325277, -0.359478},
            {0.109353, 0.993977, -0.007193, -2.394019},
            {-0.322591, 0.042333, 0.945592, -0.04139},
            {0.0, 0.0, 0.0, 1.0}
        };
        float    SDF_1_0_0_3__root_head_root_ears_ear_base_R__2_Radius;
        float    SDF_1_0_1__root_head_root_head__1_BlendFactor;
        float4x4 SDF_1_0_1_0__root_head_root_head_q__1_SpaceTransform = {
            {0.940201, -0.101083, 0.325277, 0.126522},
            {0.109353, 0.993977, -0.007193, -1.359352},
            {-0.322591, 0.042333, 0.945592, 0.400848},
            {0.0, 0.0, 0.0, 1.0}
        };
        float    SDF_1_0_1_0__root_head_root_head_q__2_Radius;
        float3   SDF_1_0_1_0__root_head_root_head_q__3_Length;
        float4x4 SDF_1_0_1_1__root_head_root_head_eyeL__1_SpaceTransform = {
            {0.976532, 0.079999, 0.199965, -0.093695},
            {0.179351, 0.211993, -0.960673, -1.368899},
            {-0.119244, 0.973991, 0.19267, -1.214508},
            {0.0, 0.0, 0.0, 1.0}
        };
        float    SDF_1_0_1_1__root_head_root_head_eyeL__2_MainRadius;
        float    SDF_1_0_1_1__root_head_root_head_eyeL__2_RingRadius;
        float    SDF_1_0_1_1__root_head_root_head_eyeL__2_Cap;
        float4x4 SDF_1_0_1_2__root_head_root_head_eyeR__1_SpaceTransform = {
            {0.729861, -0.405445, 0.550379, 0.579188},
            {0.663244, 0.225006, -0.713779, -1.402801},
            {0.165559, 0.885995, 0.433132, -1.010654},
            {0.0, 0.0, 0.0, 1.0}
        };
        float    SDF_1_0_1_2__root_head_root_head_eyeR__2_MainRadius;
        float    SDF_1_0_1_2__root_head_root_head_eyeR__2_RingRadius;
        float    SDF_1_0_1_2__root_head_root_head_eyeR__2_Cap;
        float4x4 SDF_1_0_1_3__root_head_root_head_mouth__1_SpaceTransform = {
            {-0.905656, 0.234605, -0.353197, -0.277689},
            {0.334109, -0.118035, -0.935114, -0.928785},
            {-0.261073, -0.964898, 0.028516, 1.421466},
            {0.0, 0.0, 0.0, 1.0}
        };
        float    SDF_1_0_1_3__root_head_root_head_mouth__2_MainRadius;
        float    SDF_1_0_1_3__root_head_root_head_mouth__2_RingRadius;
        float    SDF_1_0_1_3__root_head_root_head_mouth__2_Cap;
        float    SDF_1_1__root_torso__1_BlendFactor;
        float4x4 SDF_1_1_0__root_torso_cylinder__1_SpaceTransform = {
            {-0.943662, -0.089787, -0.318498, 0.12552},
            {-0.138334, 0.981387, 0.133201, -0.18457},
            {0.30061, 0.169756, -0.938518, -0.489426},
            {0.0, 0.0, 0.0, 1.0}
        };
        float    SDF_1_1_0__root_torso_cylinder__2_Height;
        float    SDF_1_1_0__root_torso_cylinder__2_Radius;
        float    SDF_1_1_0__root_torso_cylinder__2_Rounding;
        float4x4 SDF_1_1_1__root_torso_belly__1_SpaceTransform = {
            {-0.401682, -0.13734, 0.905422, 0.710692},
            {0.174445, 0.959112, 0.222875, 0.535888},
            {-0.899011, 0.247471, -0.3613, 0.104995},
            {0.0, 0.0, 0.0, 1.0}
        };
        float    SDF_1_1_1__root_torso_belly__2_Radius;
        float4x4 SDF_1_1_2__root_torso_torus__1_SpaceTransform = {
            {-0.771663, -0.203097, -0.602733, -1.085366},
            {-0.540898, -0.289, 0.789879, 0.618359},
            {-0.334612, 0.935538, 0.113155, 0.396289},
            {0.0, 0.0, 0.0, 1.0}
        };
        float    SDF_1_1_2__root_torso_torus__2_MainRadius;
        float    SDF_1_1_2__root_torso_torus__2_RingRadius;
        float    SDF_1_1_2__root_torso_torus__2_Cap;
        float4x4 SDF_1_1_3__root_torso_torus__2___1_SpaceTransform = {
            {-0.024234, -0.985158, 0.169927, -1.15107},
            {-0.799059, 0.121235, 0.588903, 0.507341},
            {-0.600764, -0.12151, -0.790138, 0.505401},
            {0.0, 0.0, 0.0, 1.0}
        };
        float    SDF_1_1_3__root_torso_torus__2___2_MainRadius;
        float    SDF_1_1_3__root_torso_torus__2___2_RingRadius;
        float    SDF_1_1_3__root_torso_torus__2___2_Cap;
        float4x4 SDF_1_1_4__root_torso_torus__1___1_SpaceTransform = {
            {-0.771663, -0.203097, -0.602733, 1.277581},
            {-0.540898, -0.289, 0.789879, 0.618574},
            {-0.334612, 0.935538, 0.113155, 0.396098},
            {0.0, 0.0, 0.0, 1.0}
        };
        float    SDF_1_1_4__root_torso_torus__1___2_MainRadius;
        float    SDF_1_1_4__root_torso_torus__1___2_RingRadius;
        float    SDF_1_1_4__root_torso_torus__1___2_Cap;
        float4x4 SDF_1_1_5__root_torso_torus__3___1_SpaceTransform = {
            {-0.807418, 0.432662, -0.401099, 1.318193},
            {0.446611, 0.892461, 0.063657, -0.374495},
            {0.385507, -0.127737, -0.91382, -0.577088},
            {0.0, 0.0, 0.0, 1.0}
        };
        float SDF_1_1_5__root_torso_torus__3___2_MainRadius;
        float SDF_1_1_5__root_torso_torus__3___2_RingRadius;
        float SDF_1_1_5__root_torso_torus__3___2_Cap;
        ENDHLSL
        Pass
        {
            HLSLPROGRAM
            float     SDF_1_0_0_0__root_head_root_ears_q__3(float3 p);
            SdfResult SDF_1_0_0_0__root_head_root_ears_q__4(float3 p);
            float     SDF_1_0_0_1__root_head_root_ears_q__3(float3 p);
            SdfResult SDF_1_0_0_1__root_head_root_ears_q__4(float3 p);
            SdfResult SDF_1_0_0_2__root_head_root_ears_ear_base_L__3(float3 p);
            SdfResult SDF_1_0_0_3__root_head_root_ears_ear_base_R__3(float3 p);
            SdfResult SDF_1_0_0__root_head_root_ears__1(float3 p);
            float     SDF_1_0_1_0__root_head_root_head_q__3(float3 p);
            SdfResult SDF_1_0_1_0__root_head_root_head_q__4(float3 p);
            SdfResult SDF_1_0_1_1__root_head_root_head_eyeL__3(float3 p);
            SdfResult SDF_1_0_1_2__root_head_root_head_eyeR__3(float3 p);
            SdfResult SDF_1_0_1_3__root_head_root_head_mouth__3(float3 p);
            SdfResult SDF_1_0_1__root_head_root_head__1(float3 p);
            SdfResult SDF_1_1_0__root_torso_cylinder__3(float3 p);
            SdfResult SDF_1_1_1__root_torso_belly__3(float3 p);
            SdfResult SDF_1_1_2__root_torso_torus__3(float3 p);
            SdfResult SDF_1_1_3__root_torso_torus__2___3(float3 p);
            SdfResult SDF_1_1_4__root_torso_torus__1___3(float3 p);
            SdfResult SDF_1_1_5__root_torso_torus__3___3(float3 p);
            SdfResult SDF_1_1__root_torso__1(float3 p);
            SdfResult SDF_1__root__1(float3 p);

            float SDF_1_0_0_0__root_head_root_ears_q__3(float3 p) {
                float3 q = abs(p) - SDF_1_0_0_0__root_head_root_ears_q__3_Length;
                return sdf::primitives3D::cylinder_rounded(
                    max(q, 0), SDF_1_0_0_0__root_head_root_ears_q__2_Radius, SDF_1_0_0_0__root_head_root_ears_q__2_Rounding, SDF_1_0_0_0__root_head_root_ears_q__2_Height
                ) + min(max(q), 0);
            }

            SdfResult SDF_1_0_0_0__root_head_root_ears_q__4(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = SDF_1_0_0_0__root_head_root_ears_q__3(sdf::operators::transform(p, SDF_1_0_0_0__root_head_root_ears_q__1_SpaceTransform));
                result.id = int4(0, 0, 0, 6);
                return result;
            }

            float SDF_1_0_0_1__root_head_root_ears_q__3(float3 p) {
                float3 q = abs(p) - SDF_1_0_0_1__root_head_root_ears_q__3_Length;
                return sdf::primitives3D::cylinder_rounded(
                    max(q, 0), SDF_1_0_0_1__root_head_root_ears_q__2_Radius, SDF_1_0_0_1__root_head_root_ears_q__2_Rounding, SDF_1_0_0_1__root_head_root_ears_q__2_Height
                ) + min(max(q), 0);
            }

            SdfResult SDF_1_0_0_1__root_head_root_ears_q__4(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = SDF_1_0_0_1__root_head_root_ears_q__3(sdf::operators::transform(p, SDF_1_0_0_1__root_head_root_ears_q__1_SpaceTransform));
                result.id = int4(0, 0, 0, 10);
                return result;
            }

            SdfResult SDF_1_0_0_2__root_head_root_ears_ear_base_L__3(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = sdf::primitives3D::sphere(
                    sdf::operators::transform(p, SDF_1_0_0_2__root_head_root_ears_ear_base_L__1_SpaceTransform), SDF_1_0_0_2__root_head_root_ears_ear_base_L__2_Radius
                );
                result.id = int4(0, 0, 0, 13);
                return result;
            }

            SdfResult SDF_1_0_0_3__root_head_root_ears_ear_base_R__3(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = sdf::primitives3D::sphere(
                    sdf::operators::transform(p, SDF_1_0_0_3__root_head_root_ears_ear_base_R__1_SpaceTransform), SDF_1_0_0_3__root_head_root_ears_ear_base_R__2_Radius
                );
                result.id = int4(0, 0, 0, 16);
                return result;
            }

            SdfResult SDF_1_0_0__root_head_root_ears__1(float3 p) {
                SdfResult result = SDF_1_0_0_0__root_head_root_ears_q__4(p);
                result = sdf::operators::unionSmooth(result, SDF_1_0_0_1__root_head_root_ears_q__4(p), SDF_1_0_0__root_head_root_ears__1_BlendFactor);
                result = sdf::operators::unionSmooth(result, SDF_1_0_0_2__root_head_root_ears_ear_base_L__3(p), SDF_1_0_0__root_head_root_ears__1_BlendFactor);
                result = sdf::operators::unionSmooth(result, SDF_1_0_0_3__root_head_root_ears_ear_base_R__3(p), SDF_1_0_0__root_head_root_ears__1_BlendFactor);
                return result;
            }

            float SDF_1_0_1_0__root_head_root_head_q__3(float3 p) {
                float3 q = abs(p) - SDF_1_0_1_0__root_head_root_head_q__3_Length;
                return sdf::primitives3D::sphere(max(q, 0), SDF_1_0_1_0__root_head_root_head_q__2_Radius) + min(max(q), 0);
            }

            SdfResult SDF_1_0_1_0__root_head_root_head_q__4(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = SDF_1_0_1_0__root_head_root_head_q__3(sdf::operators::transform(p, SDF_1_0_1_0__root_head_root_head_q__1_SpaceTransform));
                result.id = int4(0, 0, 0, 21);
                return result;
            }

            SdfResult SDF_1_0_1_1__root_head_root_head_eyeL__3(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = -sdf::primitives3D::torus_capped(
                    sdf::operators::transform(p, SDF_1_0_1_1__root_head_root_head_eyeL__1_SpaceTransform), SDF_1_0_1_1__root_head_root_head_eyeL__2_MainRadius,
                    SDF_1_0_1_1__root_head_root_head_eyeL__2_RingRadius, SDF_1_0_1_1__root_head_root_head_eyeL__2_Cap
                );
                result.id = int4(0, 0, 0, 24);
                return result;
            }

            SdfResult SDF_1_0_1_2__root_head_root_head_eyeR__3(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = -sdf::primitives3D::torus_capped(
                    sdf::operators::transform(p, SDF_1_0_1_2__root_head_root_head_eyeR__1_SpaceTransform), SDF_1_0_1_2__root_head_root_head_eyeR__2_MainRadius,
                    SDF_1_0_1_2__root_head_root_head_eyeR__2_RingRadius, SDF_1_0_1_2__root_head_root_head_eyeR__2_Cap
                );
                result.id = int4(0, 0, 0, 27);
                return result;
            }

            SdfResult SDF_1_0_1_3__root_head_root_head_mouth__3(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = -sdf::primitives3D::torus_capped(
                    sdf::operators::transform(p, SDF_1_0_1_3__root_head_root_head_mouth__1_SpaceTransform), SDF_1_0_1_3__root_head_root_head_mouth__2_MainRadius,
                    SDF_1_0_1_3__root_head_root_head_mouth__2_RingRadius, SDF_1_0_1_3__root_head_root_head_mouth__2_Cap
                );
                result.id = int4(0, 0, 0, 30);
                return result;
            }

            SdfResult SDF_1_0_1__root_head_root_head__1(float3 p) {
                SdfResult result = SDF_1_0_1_0__root_head_root_head_q__4(p);
                result = sdf::operators::intersectSimple(result, SDF_1_0_1_1__root_head_root_head_eyeL__3(p));
                result = sdf::operators::intersectSimple(result, SDF_1_0_1_2__root_head_root_head_eyeR__3(p));
                result = sdf::operators::intersectSimple(result, SDF_1_0_1_3__root_head_root_head_mouth__3(p));
                return result;
            }

            SdfResult SDF_1_1_0__root_torso_cylinder__3(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = sdf::primitives3D::cylinder_capped(
                    sdf::operators::transform(p, SDF_1_1_0__root_torso_cylinder__1_SpaceTransform), SDF_1_1_0__root_torso_cylinder__2_Height,
                    SDF_1_1_0__root_torso_cylinder__2_Radius
                );
                result.id = int4(0, 0, 0, 34);
                return result;
            }

            SdfResult SDF_1_1_1__root_torso_belly__3(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = sdf::primitives3D::sphere(sdf::operators::transform(p, SDF_1_1_1__root_torso_belly__1_SpaceTransform), SDF_1_1_1__root_torso_belly__2_Radius);
                result.id = int4(0, 0, 0, 37);
                return result;
            }

            SdfResult SDF_1_1_2__root_torso_torus__3(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = sdf::primitives3D::torus_capped(
                    sdf::operators::transform(p, SDF_1_1_2__root_torso_torus__1_SpaceTransform), SDF_1_1_2__root_torso_torus__2_MainRadius,
                    SDF_1_1_2__root_torso_torus__2_RingRadius, SDF_1_1_2__root_torso_torus__2_Cap
                );
                result.id = int4(0, 0, 0, 40);
                return result;
            }

            SdfResult SDF_1_1_3__root_torso_torus__2___3(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = sdf::primitives3D::torus_capped(
                    sdf::operators::transform(p, SDF_1_1_3__root_torso_torus__2___1_SpaceTransform), SDF_1_1_3__root_torso_torus__2___2_MainRadius,
                    SDF_1_1_3__root_torso_torus__2___2_RingRadius, SDF_1_1_3__root_torso_torus__2___2_Cap
                );
                result.id = int4(0, 0, 0, 43);
                return result;
            }

            SdfResult SDF_1_1_4__root_torso_torus__1___3(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = sdf::primitives3D::torus_capped(
                    sdf::operators::transform(p, SDF_1_1_4__root_torso_torus__1___1_SpaceTransform), SDF_1_1_4__root_torso_torus__1___2_MainRadius,
                    SDF_1_1_4__root_torso_torus__1___2_RingRadius, SDF_1_1_4__root_torso_torus__1___2_Cap
                );
                result.id = int4(0, 0, 0, 46);
                return result;
            }

            SdfResult SDF_1_1_5__root_torso_torus__3___3(float3 p) {
                SdfResult result = (SdfResult)0;
                result.distance = sdf::primitives3D::torus_capped(
                    sdf::operators::transform(p, SDF_1_1_5__root_torso_torus__3___1_SpaceTransform), SDF_1_1_5__root_torso_torus__3___2_MainRadius,
                    SDF_1_1_5__root_torso_torus__3___2_RingRadius, SDF_1_1_5__root_torso_torus__3___2_Cap
                );
                result.id = int4(0, 0, 0, 49);
                return result;
            }

            SdfResult SDF_1_1__root_torso__1(float3 p) {
                SdfResult result = SDF_1_1_0__root_torso_cylinder__3(p);
                result = sdf::operators::unionSmooth(result, SDF_1_1_1__root_torso_belly__3(p), SDF_1_1__root_torso__1_BlendFactor);
                result = sdf::operators::unionSmooth(result, SDF_1_1_2__root_torso_torus__3(p), SDF_1_1__root_torso__1_BlendFactor);
                result = sdf::operators::unionSmooth(result, SDF_1_1_3__root_torso_torus__2___3(p), SDF_1_1__root_torso__1_BlendFactor);
                result = sdf::operators::unionSmooth(result, SDF_1_1_4__root_torso_torus__1___3(p), SDF_1_1__root_torso__1_BlendFactor);
                result = sdf::operators::unionSmooth(result, SDF_1_1_5__root_torso_torus__3___3(p), SDF_1_1__root_torso__1_BlendFactor);
                return result;
            }

            SdfResult SDF_1__root__1(float3 p) {
                SdfResult result = SDF_1_0_0__root_head_root_ears__1(p);
                result = sdf::operators::unionSmooth(result, SDF_1_0_1__root_head_root_head__1(p), SDF_1__root__1_BlendFactor);
                result = sdf::operators::unionSmooth(result, SDF_1_1__root_torso__1(p), SDF_1__root__1_BlendFactor);
                return result;
            }

            SdfResult sdfScene(float3 p) {
                return SDF_1__root__1(p);
            }
            ENDHLSL
        }
    }
}