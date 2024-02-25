// GENERATED SHADER CONTENT. ANY MODIFICATIONS WILL BE OVERWRITTEN
Shader "big encompassing domain (generated)" {
    Properties {
        [Header(global raymarching properties)]
        [Space]
        [Enum(UnityEngine.Rendering.CullMode)]
        _Cull("Cull", Int) = 2
        [Tooltip(Enable for correct blending with other geometry and backface rendering)]
        [Toggle]
        [KeyEnum(Off, On)]
        _ZWrite("ZWrite", Float) = 1
        [Enum(UnityEngine.Rendering.CompareFunction)]
        _ZTest("ZTest", Int) = 4
        [Header(box)]
        sdfScene_box_0_boxsize("Box size", Vector) =(0.25, 0.25, 0.25, 0.00)
        [Header(box)]
        sdfScene_box_1_boxsize("Box size", Vector) =(0.25, 0.25, 0.25, 0.00)
    }
    Fallback "Sdf/Fallback"
    CustomEditor "me.tooster.sdf.Editor.Controllers.Editors.SdfSceneShaderEditor"
    SubShader {
        Tags {
            "RenderType" = "Opaque"
            "Queue" = "Geometry+1"
            "IgnoreProjector" = "True"
            "LightMode" = "ForwardBase"
        }
        ZTest [_ZTest]
        Cull [_Cull]
        ZWrite [_ZWrite]
        Pass {
            HLSLPROGRAM
            #pragma target 5.0 
            #include "UnityCG.cginc" 
            #include "Packages/me.tooster.sdf/Editor/Resources/Includes/raymarching.hlsl" 
            #include "Packages/me.tooster.sdf/Editor/Resources/Includes/util.hlsl" 
            #include "Packages/me.tooster.sdf/Editor/Resources/Includes/matrix.hlsl" 
            #include "Packages/me.tooster.sdf/Editor/Resources/Includes/operators.hlsl" 
            #include "Packages/me.tooster.sdf/Editor/Resources/Includes/noise.hlsl" 
            #include "Packages/me.tooster.sdf/Editor/Resources/Includes/primitives.hlsl" 
            #pragma vertex vertexShader 
            #pragma fragment fragmentShader 
            float4x4 sdfScene_box_0_transform;
            float3 sdfScene_box_0_boxsize;
            float4x4 sdfScene_box_1_transform;
            float3 sdfScene_box_1_boxsize;
            SdfResult sdfScene_box_0(float3 p) {
                SdfResult result = (SdfResult) 0;
                p = sdf::operators::transform(p, sdfScene_box_0_transform);
                result.distance = sdf::primitives3D::box(p, sdfScene_box_0_boxsize);
                result.id = 0;
                return result;
            }
            SdfResult sdfScene_box_1(float3 p) {
                SdfResult result = (SdfResult) 0;
                p = sdf::operators::transform(p, sdfScene_box_1_transform);
                result.distance = sdf::primitives3D::box(p, sdfScene_box_1_boxsize);
                result.id = 1;
                return result;
            }
            SdfResult sdfScene(float3 p) {
                return sdf::operators::unionSimple(sdfScene_box_0(p), sdfScene_box_1(p));
            }
            ENDHLSL
        }
    }
}
