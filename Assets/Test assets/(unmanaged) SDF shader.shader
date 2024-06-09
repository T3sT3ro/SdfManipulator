// GENERATED SHADER CONTENT. ANY MODIFICATIONS WILL BE OVERWRITTEN
Shader "(unmanaged) Box SDF Scene"
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
        [Header(combine_sphere_2)]
        _0_0_combine_sphere_2_radius("Sphere radius", Float) = 1.00
        [Header(combine_sphere_1)]
        _0_1_combine_sphere_1_radius("Sphere radius", Float) = 1.00
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
            float4x4 _0_combine_transform = MATRIX_ID;
            float    _0_combine_blendFactor;
            float4x4 _0_0_combine_sphere_2_transform = MATRIX_ID;
            float    _0_0_combine_sphere_2_radius;
            float4x4 _0_1_combine_sphere_1_transform = MATRIX_ID;
            float    _0_1_combine_sphere_1_radius;

            SdfResult _0_0_combine_sphere_2(float3 p) {
                SdfResult result = (SdfResult)0;
                p = sdf::operators::transform(p, _0_0_combine_sphere_2_transform);
                result.distance = sdf::primitives3D::sphere(p, _0_0_combine_sphere_2_radius);
                result.id = 1;
                return result;
            }

            SdfResult _0_1_combine_sphere_1(float3 p) {
                SdfResult result = (SdfResult)0;
                p = sdf::operators::transform(p, _0_1_combine_sphere_1_transform);
                result.distance = sdf::primitives3D::sphere(p, _0_1_combine_sphere_1_radius);
                result.id = 2;
                return result;
            }

            SdfResult sdfScene(float3 p) {
                return sdf::operators::unionSmooth(_0_0_combine_sphere_2(p), _0_1_combine_sphere_1(p), _0_combine_blendFactor);
            }
            ENDHLSL
        }
    }
}