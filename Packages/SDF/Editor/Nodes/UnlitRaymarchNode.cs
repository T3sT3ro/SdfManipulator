using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Editor.Nodes.Ports;
using SDF.Interface;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;

namespace UnityEditor.ShaderGraph.Internal {
    public class UnlitRaymarchNode : ISdfMasterNode {
        private List<IPropertyProvider> Properties { get; }
        private SdfPort                 Sdf;

        public string ShaderCode =>
            @$"""
Shader ""SDF/Unlit""
{{
    // PROP BLOCK
    Properties
    {{
        [Header(Shader properties)]
        [Enum(UnityEngine.Rendering.CullMode)] _Cull (""Cull"", Int) = 0
        [KeywordEnum(Less, LEqual, Equal, GEqual, Greater, NotEqual, Always)] _ZTest(""ZTest"", Int) = 0

        [Header(Raymarcher)]
        _MAX_STEPS (""max raymarching steps"", Int) = 200
        _MAX_DISTANCE (""max raymarching distance"", Float) = 200.0
        _RAY_ORIGIN_BIAS (""ray origin bias"", Float) = 0
        _EPSILON_RAY (""epsilon step for ray to consider hit"", Float) = 0.001
        _EPSILON_NORMAL (""epsilon for calculating normal"", Float) = 0.001

        [KeywordEnum(Near, Face)] _RayOrigin(""Ray origin"", Int) = 0
        [KeywordEnum(World, Local)] _Origin(""Scene origin"", Int) = 0
        [Tooltip(Only works for origin type local)]
        [Toggle] _PRESERVE_SCALE (""Preserve local scale"", Int) = 1

        [Header(SDF Scene)]
  
        {Properties.Select(p => p.Properties.Select(p => p.ShaderlabBlock))}
    }}

    // Fallback ""Diffuse""

    SubShader
    {{
        Tags
        {{
            ""RenderType""=""Geometry""
            ""Queue""=""Geometry+1"" // +1 to resolve artifacts when using Cull Off
            ""IgnoreProjector""=""True""
        }}
        //        Blend SrcAlpha OneMinusSrcAlpha
        ZTest [_ZTest] // Can be customized manually, but when meshes intersect SDFs, this helps to draw both properly  
        Cull [_Cull] // Draw camera inside a domain
        ZWrite [_ZWrite]

        // common includes for all passes
        HLSLINCLUDE

        #pragma target 5.0
        #pragma vertex vert
        #pragma fragment frag
        #pragma shader_feature_local _ORIGIN_WORLD _ORIGIN_LOCAL
        #pragma shader_feature_local _RAYORIGIN_NEAR _RAYORIGIN_FACE
        #pragma shader_feature_local _DRAWMODE_MATERIAL _DRAWMODE_ALBEDO _DRAWMODE_TEXTURE _DRAWMODE_NORMALLOCAL \
            _DRAWMODE_NORMALWORLD _DRAWMODE_ID _DRAWMODE_STEPS _DRAWMODE_DEPTH
        #pragma shader_feature_local _SCALE_INVARIANT
        #pragma shader_feature_local _ZWRITE_ON _ZWRITE_OFF
        // #pragma shader_feature_local _SCENEVIEW

        #include ""UnityCG.cginc""
        #include ""Assets/SDF/Includes/primitives.cginc""
        #include ""Assets/SDF/Includes/operators.cginc""
        #include ""Assets/SDF/Includes/noise.cginc""
        #include ""Assets/SDF/Includes/types.cginc""
        #include ""Assets/SDF/Includes/util.cginc""
        #include ""Assets/SDF/Includes/matrix.cginc"" // FIXME: inverse might be a problem

        #pragma shader_feature_local _PRESERVE_SCALE_ON
        static const float4x4 SCALE_MATRIX =
            #if defined(_PRESERVE_SCALE_ON) && defined(_ORIGIN_LOCAL)
            {{
                {{length(float3(UNITY_MATRIX_M[0].x, UNITY_MATRIX_M[1].x, UNITY_MATRIX_M[2].x)), 0, 0, 0}},
                {{0, length(float3(UNITY_MATRIX_M[0].y, UNITY_MATRIX_M[1].y, UNITY_MATRIX_M[2].y)), 0, 0}},
                {{0, 0, length(float3(UNITY_MATRIX_M[0].z, UNITY_MATRIX_M[1].z, UNITY_MATRIX_M[2].z)), 0}},
                {{0, 0, 0, 1}}
            }};
            #else
            MATRIX_ID;
        #endif

        static const float4x4 SCALE_MATRIX_I = inverse(SCALE_MATRIX);
        
        // https://gist.github.com/unitycoder/c5847a82343a8e721035
        // static const float3 camera_forward = UNITY_MATRIX_IT_MV[2].xyz;
        static const float4x4 inv = mul(SCALE_MATRIX, inverse(
                                            #ifdef _ORIGIN_WORLD
                                            UNITY_MATRIX_VP
                                            #else
                                            UNITY_MATRIX_MVP
                                            #endif
                                        ));

        UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);

        float _EPSILON_RAY;
        float _EPSILON_NORMAL;
        float _MAX_DISTANCE;
        float _RAY_ORIGIN_BIAS;
        float _MAX_STEPS;

        {Properties.Select(provider => provider.GetType().GetMethod("GetPropertyBlockString",
            BindingFlags.NonPublic | BindingFlags.Instance).Invoke(this, null))}

        ENDHLSL

        Pass
        {{
            HLSLPROGRAM
            v2f vert(appdata_base v)
            {{
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex); // clip space
                o.screenPos = ComputeScreenPos(o.vertex); // from 0,0 to 1,1
                o.hitpos = v.vertex;
                COMPUTE_EYEDEPTH(o.screenPos.z);
                return o;
            }}

            // =======================================================================
            Hit __SDF(float3 p)
            {{
                {Sdf}
            }}

            // -----------------------------------------------------------------------

            void castRay(inout RayInfo3D ray, in float max_distance)
            {{
                float d = ray.hit.distance;
                Hit hit = {{_MAX_DISTANCE, NO_ID}};
                ray.hit = hit;
                for (ray.steps = 0; ray.steps < _MAX_STEPS; ray.steps++)
                {{
                    if (d >= _MAX_DISTANCE || d >= max_distance)
                        return;

                    ray.p = ray.ro + d * ray.rd;

                    Hit hit = __SDF(ray.p);
                    if (hit.distance < _EPSILON_RAY)
                    {{
                        ray.hit.distance = d;
                        ray.hit.id = hit.id;
                        return;
                    }}

                    d += hit.distance;
                }}
            }}

            // =======================================================================

            fixed4 frag(v2f i, fixed facing : VFACE) : SV_TARGET0
            {{
                const float3 screenPos = i.screenPos.xyz / i.screenPos.w; // 0,0 to 1,1 on screen

                RayInfo3D ray = (RayInfo3D)0;

                // NDC from (-1, -1, -1) to (1, 1, 1) 
                float3 NDC = 2. * screenPos.xyz - 1.;

                float4 ro = mul(inv, float4(NDC.xy, UNITY_NEAR_CLIP_VALUE, 1)); // ray origin on near plane
                ro /= ro.w;
                #ifdef _RAYORIGIN_NEAR
                #else
                {{
                    float4 rs =
                        #ifdef _ORIGIN_WORLD
                        mul(UNITY_MATRIX_M, float4(i.hitpos, 1));
                        #else
                        fixed4(i.hitpos, 1);
                    #endif
                    ray.hit.distance = distance(mul(rs, SCALE_MATRIX), ro); // start on ray
                }}
                #endif

                float4 re = mul(inv, float4(NDC.xy, 1, 1)); // ray end on far plane
                re /= re.w;
                float3 rd = normalize((re - ro).xyz); // ray direction


                ray.ro = ro; // in object space
                ray.rd = rd; // in object space

                ray.hit.distance += _RAY_ORIGIN_BIAS;

                // read camera depth texture to correctly blend with scene geometry
                // beware, that _CameraDepthTexture IS NOT the depth buffer!
                // it is populated in the prepass and doesn't change in subsequent passes
                // https://forum.unity.com/threads/does-depth-buffer-update-between-passes.620575/
                float camDepth = CorrectDepth(tex2D(_CameraDepthTexture, screenPos.xy).rg);

                float4 forward = mul(inv, float4(0, 0, 1, 1)); // ray end on far plane
                forward /= forward.w;
                forward = normalize(forward); // forward in object space

                castRay(ray, camDepth / dot(forward, rd));


                clip(ray.hit.id); // discard rays without hit

                fixed4 color_material = __MATERIAL(ray.hit.id); // color

                return color_material;
            }}

            // =======================================================================
            ENDHLSL
        }} // End Pass

        // Pass {{}}
    }}
}}
""";
    }
}
