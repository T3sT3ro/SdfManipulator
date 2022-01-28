Shader "SDF/Domain"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"
        }
        LOD 100
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha


        Pass
        {
            HLSLPROGRAM
            #pragma target 5.0
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "HLSLSupport.cginc"
            #include "Assets/SDF/Includes/primitives.cginc"
            #include "Assets/SDF/Includes/types.cginc"
            #include "Assets/SDF/Includes/matrix.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv: TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            // -----------------------------------

            #define MAX_STEPS 1000
            #define MAX_DISTANCE 2000.0f
            #define EPSILON 0.001f
            #define BACKGROUND float4(0, 0, 0, 0.5)
            #define NO_ID -1

            // -----------------------------------


            Hit __SDF(float3 p)
            {
                p -= float3(.2,.5,.5);
                float4x4 rot = m_rotate(.15, float3(0,0,1));
                p = mul(rot, p);
                //p = mul(float4(p, 1), float4x4 {{0}, {0}, {0}, {0}}).xyz;
                Hit ret = {sdf::primitives3D::torus(p, 1, .1), 1};
                return ret;
            }


            // https://iquilezles.org/www/articles/normalsSDF/normalsSDF.htm
            float3 __SDF_NORMAL(float3 p)
            {
                // using tetrahedron technique
                // EPSILON -- can be adjusted using pixel footprint
                const float2 k = float2(1, -1);
                return normalize(
                    k.xyy * __SDF(p + k.xyy * EPSILON).distance +
                    k.yyx * __SDF(p + k.yyx * EPSILON).distance +
                    k.yxy * __SDF(p + k.yxy * EPSILON).distance +
                    k.xxx * __SDF(p + k.xxx * EPSILON).distance
                );
            }

            float3 __MATERIAL(int id)
            {
                switch (id)
                {
                case 1:
                    return float3(0.2, 0, 0); // RED albedo
                default:
                    return float3(1, 0, 1); // MAGENTA
                }
            }

            void castRay(inout RayInfo3D ray)
            {
                Hit hit = {MAX_DISTANCE, NO_ID};
                ray.hit = hit;

                float d = 0;
                for (ray.steps = 0; ray.steps < MAX_STEPS; ray.steps++)
                {
                    ray.p = ray.ro + d * ray.rd;

                    const Hit hit = __SDF(ray.p);
                    if (hit.distance < EPSILON)
                    {
                        ray.hit.distance = d;
                        ray.hit.id = hit.id;
                        break;
                    }

                    d += hit.distance;

                    if (d >= MAX_DISTANCE)
                        break;
                }
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv - .5;

                RayInfo3D ray;

                ray.ro = float3(0, 1, -3);
                ray.rd = normalize(float3(uv.x, uv.y - .3, 1));

                castRay(ray);

                if (ray.hit.id == NO_ID) // exit if nothing hit
                    return fixed4(0, 0, 0, 0);

                const float3 n = __SDF_NORMAL(ray.p);

                fixed3 col = normalize(n) * .5 + .5;

                return fixed4(col, 1);
            }

            // -----------------------------------
            ENDHLSL
        }
    }
}