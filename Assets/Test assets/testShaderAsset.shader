// GENERATED SHADER CONTENT. ANY MODIFICATIONS WILL BE OVERWRITTEN
Shader "Box SDF Scene (generated)"
{
    Properties
    {
        [Header(global_raymarching_properties)]
        [Space]
        [Enum(UnityEngine.Rendering.CullMode)]
        _Cull("Cull", Int) = 2
        [Tooltip(Enable for correct blending with other geometry and backface rendering)]
        [Toggle]
        [KeyEnum(Off, On)]
        _ZWrite("ZWrite", Float) = 1
        [Enum(UnityEngine.Rendering.CompareFunction)]
        _ZTest("ZTest", Int) = 4
        [Header(combine)]
        _0_combine_blendFactor("Blend factor", Float) = 1.00
        [Header(combine_sphere_1)]
        _0_0_combine_sphere_1_radius("Sphere radius", Float) = 1.00
        [Header(combine_sphere_2)]
        _0_1_combine_sphere_2_radius("Sphere radius", Float) = 1.00
        [Header(combine_intersect)]
        _0_2_combine_intersect_blendFactor("Blend factor", Float) = 1.00
        [Header(combine_intersect_sphere)]
        _0_2_0_combine_intersect_sphere_radius("Sphere radius", Float) = 1.00
        [Header(combine_intersect_box)]
        _0_2_1_combine_intersect_box_boxsize("Box size", Vector) =(0.25, 0.25, 0.25, 0.00)
        [Header(combine_smooth)]
        _0_3_combine_smooth_blendFactor("Blend factor", Float) = 1.00
        [Header(combine_smooth_sphere)]
        _0_3_0_combine_smooth_sphere_radius("Sphere radius", Float) = 1.00
        [Header(combine_smooth_box)]
        _0_3_1_combine_smooth_box_boxsize("Box size", Vector) =(0.25, 0.25, 0.25, 0.00)
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
            #include "Packages/me.tooster.sdf/Editor/Resources/Includes/primitives.hlsl"
            #include "Packages/me.tooster.sdf/Editor/Resources/Includes/debugBaseShading.hlsl"

            #pragma vertex vertexShader
            #pragma fragment fragmentShader
            float4x4 _0_combine_transform;
            float    _0_combine_blendFactor;
            float4x4 _0_0_combine_sphere_1_transform;
            float    _0_0_combine_sphere_1_radius;
            float4x4 _0_1_combine_sphere_2_transform;
            float    _0_1_combine_sphere_2_radius;
            float4x4 _0_2_combine_intersect_transform;
            float    _0_2_combine_intersect_blendFactor;
            float4x4 _0_2_0_combine_intersect_sphere_transform;
            float    _0_2_0_combine_intersect_sphere_radius;
            float4x4 _0_2_1_combine_intersect_box_transform;
            float3   _0_2_1_combine_intersect_box_boxsize;
            float4x4 _0_3_combine_smooth_transform;
            float    _0_3_combine_smooth_blendFactor;
            float4x4 _0_3_0_combine_smooth_sphere_transform;
            float    _0_3_0_combine_smooth_sphere_radius;
            float4x4 _0_3_1_combine_smooth_box_transform;
            float3   _0_3_1_combine_smooth_box_boxsize;

            SdfResult _0_0_combine_sphere_1(float3 p) {
                SdfResult result = (SdfResult)0;
                p = sdf::operators::transform(p, _0_0_combine_sphere_1_transform);
                result.distance = sdf::primitives3D::sphere(p, _0_0_combine_sphere_1_radius);
                result.id = 1;
                return result;
            }

            SdfResult _0_1_combine_sphere_2(float3 p) {
                SdfResult result = (SdfResult)0;
                p = sdf::operators::transform(p, _0_1_combine_sphere_2_transform);
                result.distance = sdf::primitives3D::sphere(p, _0_1_combine_sphere_2_radius);
                result.id = 2;
                return result;
            }

            SdfResult _0_2_0_combine_intersect_sphere(float3 p) {
                SdfResult result = (SdfResult)0;
                p = sdf::operators::transform(p, _0_2_0_combine_intersect_sphere_transform);
                result.distance = sdf::primitives3D::sphere(p, _0_2_0_combine_intersect_sphere_radius);
                result.id = 4;
                return result;
            }

            SdfResult _0_2_1_combine_intersect_box(float3 p) {
                SdfResult result = (SdfResult)0;
                p = sdf::operators::transform(p, _0_2_1_combine_intersect_box_transform);
                result.distance = sdf::primitives3D::box(p, _0_2_1_combine_intersect_box_boxsize);
                result.id = 5;
                return result;
            }

            SdfResult _0_3_0_combine_smooth_sphere(float3 p) {
                SdfResult result = (SdfResult)0;
                p = sdf::operators::transform(p, _0_3_0_combine_smooth_sphere_transform);
                result.distance = sdf::primitives3D::sphere(p, _0_3_0_combine_smooth_sphere_radius);
                result.id = 7;
                return result;
            }

            SdfResult _0_3_1_combine_smooth_box(float3 p) {
                SdfResult result = (SdfResult)0;
                p = sdf::operators::transform(p, _0_3_1_combine_smooth_box_transform);
                result.distance = sdf::primitives3D::box(p, _0_3_1_combine_smooth_box_boxsize);
                result.id = 8;
                return result;
            }

            SdfResult sdfScene(float3 p) {
                return sdf::operators::unionSimple(
                    sdf::operators::unionSimple(_0_0_combine_sphere_1(p), _0_1_combine_sphere_2(p)), sdf::operators::unionSimple(
                        sdf::operators::intersectSimple(_0_2_0_combine_intersect_sphere(p), _0_2_1_combine_intersect_box(p)), sdf::operators::unionSmooth(
                            _0_3_0_combine_smooth_sphere(p), _0_3_1_combine_smooth_box(p), _0_3_combine_smooth_blendFactor
                        )
                    )
                );
            }
            ENDHLSL
        }
    }
}