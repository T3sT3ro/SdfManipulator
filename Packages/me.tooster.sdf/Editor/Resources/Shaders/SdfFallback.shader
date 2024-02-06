Shader "Sdf/Fallback"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 vert(in float4 pos : POSITION, out float4 screenPos : TEXCOORD1) : SV_POSITION
            {
                float4 clipPos = UnityObjectToClipPos(pos);
                screenPos = ComputeScreenPos(clipPos);
                return clipPos;
            }

            fixed4 frag(in float4 screenPos : TEXCOORD1) : SV_Target   
            {
                screenPos.xy = trunc(screenPos.xy / screenPos.w * _ScreenParams.xy / 5);

                clip((screenPos.x + screenPos.y) % 2 == 0 ? -1 : 0);
                return fixed4(1.0, .2, 0.0, 1.0);
            }
            ENDHLSL
        }
    }
}