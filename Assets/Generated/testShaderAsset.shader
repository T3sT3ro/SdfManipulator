// GENERATED SHADER CONTENT. ANY MODIFICATIONS WILL BE OVERWRITTEN
Shader "Box SDF Scene (generated)" {
    Properties {
        [Header(global raymarching properties)]
        [Space]
        [Enum(UnityEngine.Rendering.CullMode)]
        _Cull("Cull", Integer) = 0
        [Tooltip(Enable to assure correct blending of multiple domains and backface rendering)]
        [Toggle]
        [KeyEnum(Off, On)]
        _ZWrite("ZWrite", Float) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)]
        _ZTest("ZTest", Integer) = 1
        [Header(raymarching properties)]
        [Space]
        sphere_1("Sphere radius", Float) = 1.00
        sphere_2("Sphere radius", Float) = 1.00
    }
    Fallback "Sdf/Fallback"
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
            #include "Packages/me.tooster.sdf/Editor/Resources/Includes/types.hlsl" 
            #include "Packages/me.tooster.sdf/Editor/Resources/Includes/util.hlsl" 
            #include "Packages/me.tooster.sdf/Editor/Resources/Includes/matrix.hlsl" 
            #include "Packages/me.tooster.sdf/Editor/Resources/Includes/operators.hlsl" 
            #include "Packages/me.tooster.sdf/Editor/Resources/Includes/noise.hlsl" 
            #include "Packages/me.tooster.sdf/Editor/Resources/Includes/raymarching.hlsl" 
            #include "Packages/me.tooster.sdf/Editor/Resources/Includes/primitives.hlsl" 
            #pragma vertex vertexShader 
            #pragma fragment fragmentShader 
            float sphere_1;
            float sphere_2;
            SdfResult sdfScene(float3 p) {
                SdfResult result = (SdfResult) 0;
                result.distance = sdf::primitives3D::sphere(p, 0.50);
                result.id = 1;
                return result;
            }
            ENDHLSL
        }
    }
}
