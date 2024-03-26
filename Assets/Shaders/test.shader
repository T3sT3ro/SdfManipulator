Shader "Unlit/test"
{
    CustomEditor "me.tooster.sdf.Editor.Controllers.Editors.SdfShaderEditor"
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            // Upgrade NOTE: excluded shader from DX11; has structs without semantics (struct v2f members uninitialized)
            #pragma exclude_renderers d3d11
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            v2f_img vert (appdata_base v)
            {
                v2f_img o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o; // warning: uninitialized member 'uv' 
            }

            fixed4 frag (v2f_img i) : SV_Target
            {
                fixed4 col = fixed4(0.1, 1, 0.1, 1);
                return col;
            }
            ENDCG
        }
    }
}
