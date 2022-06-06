Shader "SDF/Domain"
{
    // PROP BLOCK
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TorusSizes ("Torus R and r", Vector) = (.5, .1, 0, 0)
        _MAX_STEPS ("max steps of raymarcher", Int) = 200
        _MAX_DISTANCE ("max distance a ray can march", Float) = 200.0
        _EPSILON_RAY ("minimum distance for ray to consider the surface hit", Float) = 0.001
        _EPSILON_NORMAL ("epsilon for claculating normal", Float) = 0.001
    }

    //    Fallback "Diffuse"

    SubShader
    {
        Tags
        {
            "RenderType"="Geometry"
            "Queue"="Geometry"
            "IgnoreProjector"="True"
        }
        //        ZWrite On
        ZTest On // When
        Cull Off // When camera is inside domain        
        // common includes for all passes
        HLSLINCLUDE
        #pragma target 5.0
        #pragma vertex vert
        #pragma fragment frag

        #include "UnityCG.cginc"
        #include "Assets/SDF/Includes/primitives.cginc"
        #include "Assets/SDF/Includes/types.cginc"
        #include "Assets/SDF/Includes/matrix.cginc" // FIXME: inverse might be a problem

        sampler2D _MainTex;
        float4 _MainTex_ST;
        float4 _TorusSizes;
        float _EPSILON_RAY;
        float _EPSILON_NORMAL;
        float _MAX_DISTANCE;
        float _MAX_STEPS;

        static const float3 viewDir = UNITY_MATRIX_IT_MV[2].xyz;
        static const float4x4 inv = inverse(UNITY_MATRIX_MVP);
        ENDHLSL

        Pass
        {
            HLSLPROGRAM
            v2f vert(appdata_base v, uint id: SV_VertexID)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex); // clip space
                return o;
            }

            // =======================================================================
            Hit __SDF(float3 p)
            {
                // float4x4 rot = m_rotate(_Time.y, float3(0, 0, 1));
                // p = mul(rot, p);
                rotZ(p, _Time.y / 3);
                Hit ret = {sdf::primitives3D::torus(p, _TorusSizes.x, _TorusSizes.y), 0};
                return ret;
            }

            // https://iquilezles.org/www/articles/normalsSDF/normalsSDF.htm
            float3 __SDF_NORMAL(float3 p)
            {
                // using tetrahedron technique
                // EPSILON -- can be adjusted using pixel footprint
                const float2 k = float2(1, -1);
                return normalize(
                    k.xyy * __SDF(p + k.xyy * _EPSILON_NORMAL).distance +
                    k.yyx * __SDF(p + k.yyx * _EPSILON_NORMAL).distance +
                    k.yxy * __SDF(p + k.yxy * _EPSILON_NORMAL).distance +
                    k.xxx * __SDF(p + k.xxx * _EPSILON_NORMAL).distance
                );
            }


            fixed4 __MATERIAL(int id)
            {
                static fixed4 _COLORS[] = {
                    fixed4(.5, 0, .5, 1), // NO_ID (magenta)
                    // ------------------- VALID MATERIALS BELOW
                    fixed4(0, .7, .2, .5), // tomato :)
                };
                return _COLORS[id + 1];
            }

            void castRay(inout RayInfo3D ray)
            {
                float d = 0;
                for (ray.steps = 0; ray.steps < _MAX_STEPS; ray.steps++)
                {
                    ray.p = ray.ro + d * ray.rd;

                    Hit hit = __SDF(ray.p);
                    if (hit.distance < _EPSILON_RAY)
                    {
                        ray.hit.distance = d;
                        ray.hit.id = hit.id;
                        break;
                    }

                    d += hit.distance;

                    if (d >= _MAX_DISTANCE)
                        break;
                }
            }

            float square(float rise, float fall, float t)
            {
                return step(rise, t) * step(t, fall);
            }

            // =======================================================================

            f2p frag(v2f i)
            {
                RayInfo3D ray = (RayInfo3D)0;
                Hit hit = {_MAX_DISTANCE, NO_ID};
                ray.hit = hit;

                // NDC from (-1, -1, -1) to (1, 1, 1) 
                float2 NDC = 2. * i.vertex.xy / _ScreenParams.xy - 1.;

                float4 ro = mul(inv, float4(NDC.xy, UNITY_NEAR_CLIP_VALUE, 1));
                ro /= ro.w;

                float4 re = mul(inv, float4(NDC.xy, 1, 1));
                re /= re.w;
                float3 rd = normalize((re - ro).xyz);

                ray.ro = ro;
                ray.rd = rd;

                castRay(ray);

                clip(ray.hit.id); // discard rays without hit
    
                float3 n = __SDF_NORMAL(ray.p);

                // GAMMA
                // col = pow(col, 0.45);
                fixed4 mat_col = __MATERIAL(ray.hit.id); // color
                fixed4 nd_col = fixed4(n * .5 + .5, 1); // domain normal color
                fixed4 nw_col = fixed4(UnityObjectToWorldNormal(n) * .5 + .5, 1); // world normal color
            
                fixed4 colors[] = {
                    {mat_col},
                    {nd_col},
                    {nw_col},
                };
                // lerp(colors[floor(t)], colors[ceil(t)], frac(t))

                f2p o = {
                    {mat_col},
                    {float3(normalize(n) * .5 + .5)},
                    ray.hit.id,
                };
                return o;
            }

            // =======================================================================
            ENDHLSL
        }
    }
}