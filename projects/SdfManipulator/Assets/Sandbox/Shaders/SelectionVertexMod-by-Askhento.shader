
Shader "Custom/SelectionVertexMod"
{
    Properties
    {
        _BaseMap ("Example Texture", 2D) = "white" { }
        _BaseColor ("Example Colour", Color) = (0, 0.66, 0.73, 1)
    }
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque" "Queue" = "Geometry" }

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        CBUFFER_START(UnityPerMaterial)
            float4 _BaseMap_ST;
            float4 _BaseColor;
        CBUFFER_END

        

        // Structs
        struct Attributes
        {
            float4 positionOS : POSITION;
            float2 uv : TEXCOORD0;
            float4 color : COLOR;
        };

        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float2 uv : TEXCOORD0;
            float4 color : COLOR;
        };
        


        // Vertex Shader
        Varyings PickPassVertex(Attributes IN)
        {
            Varyings OUT;

            // IN.positionOS.xyz *= 2.0;
            IN.positionOS.x += 1.0;


            VertexPositionInputs positionInputs = GetVertexPositionInputs(IN.positionOS.xyz);
            OUT.positionCS = positionInputs.positionCS;



            // Or :
            //OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
            OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
            OUT.color = IN.color;
            return OUT;
        }


        ENDHLSL

        Pass
        {
            Name "Unlit"

            HLSLPROGRAM
            #pragma vertex PickPassVertex
            #pragma fragment UnlitPassFragment



            // Textures, Samplers & Global Properties
            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);



            // Fragment Shader
            half4 UnlitPassFragment(Varyings IN) : SV_Target
            {
                half4 baseMap = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
                return baseMap * _BaseColor * IN.color;
            }
            ENDHLSL
        }

        Pass
        {
            Name "SceneSelectionPass"
            Tags { "LightMode" = "SceneSelectionPass" }

            // Render State
            Cull Off

            HLSLPROGRAM

            #define SCENESELECTIONPASS 1


            #pragma vertex PickPassVertex
            #pragma fragment frag


            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
                float4 _SelectionID;
            #endif

            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
                int _ObjectId;
                int _PassValue;
            #endif


            half4 frag() : SV_TARGET
            {
                half4 outColor = 0;
                #ifdef SCENESELECTIONPASS
                    // We use depth prepass for scene selection in the editor, this code allow to output the outline correctly
                    outColor = half4(_ObjectId, _PassValue, 1.0, 1.0);
                #elif defined(SCENEPICKINGPASS)
                    outColor = _SelectionID;
                #endif

                return outColor;
            }

            ENDHLSL
        }

        Pass
        {
            Name "ScenePickingPass"
            Tags { "LightMode" = "Picking" }

            // Render State
            Cull Back

            HLSLPROGRAM

            #define SCENEPICKINGPASS 1


            #pragma vertex PickPassVertex
            #pragma fragment frag


            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
                float4 _SelectionID;
            #endif

            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
                int _ObjectId;
                int _PassValue;
            #endif



            half4 frag() : SV_TARGET
            {
                half4 outColor = 0;
                #ifdef SCENESELECTIONPASS
                    // We use depth prepass for scene selection in the editor, this code allow to output the outline correctly
                    outColor = half4(_ObjectId, _PassValue, 1.0, 1.0);
                #elif defined(SCENEPICKINGPASS)
                    outColor = _SelectionID;
                #endif

                return outColor;
            }

            ENDHLSL
        }
    }
}
