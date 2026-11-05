Pass
{
    Tags {"LightMode"="SceneSelectionPass"}
    
    HLSLPROGRAM
    #pragma vertex vert
    #pragma fragment frag
    #pragma target 3.0
    #include "UnityCG.cginc"

    struct appdata
    {
        float4 vertex : POSITION;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    struct v2f
    {
        float4 pos : SV_POSITION;
        float4 screenPos : TEXCOORD0;
        UNITY_VERTEX_OUTPUT_STEREO
    };

    v2f vert(appdata v)
    {
        v2f o;
        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
        o.pos = UnityObjectToClipPos(v.vertex);
        o.screenPos = ComputeScreenPos(o.pos);
        return o;
    }

    float sdSphere(float3 p, float s)
    {
        return length(p) - s;
    }

    float map(float3 p)
    {
        // Example: A sphere at the origin
        return sdSphere(p, 0.5);
    }

    float3 calcNormal(in float3 p)
    {
        const float h = 0.0001;
        const float2 k = float2(1, -1);
        return normalize(
            k.xyy * map(p + k.xyy * h) +
            k.yyx * map(p + k.yyx * h) +
            k.yxy * map(p + k.yxy * h) +
            k.xxx * map(p + k.xxx * h)
        );
    }

    float4 frag(v2f i) : SV_Target
    {
        float3 ro = _WorldSpaceCameraPos;
        float3 rd = normalize(i.screenPos.xyz / i.screenPos.w - ro);

        float t = 0;
        float3 p;

        for (int i = 0; i < 100; i++)
        {
            p = ro + rd * t;
            float d = map(p);
            if (d < 0.001 || t > 100) break;
            t += d;
        }

        if (t > 100) discard;

        float3 n = calcNormal(p);
        float3 objectPos = mul(unity_WorldToObject, float4(p, 1)).xyz;
        
        // Output object space position for picking
        return float4(objectPos, 1);
    }
    ENDHLSL
}