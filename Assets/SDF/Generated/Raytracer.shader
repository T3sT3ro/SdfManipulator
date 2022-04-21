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
        //        Cull Off // When camera is inside domain        
        // common includes for all passes
        HLSLINCLUDE
        #pragma target 5.0
        #pragma vertex vert
        #pragma fragment frag

        #include "UnityCG.cginc"
        #include "HLSLSupport.cginc"
        #include "Assets/SDF/Includes/primitives.cginc"
        #include "Assets/SDF/Includes/types.cginc"
        #include "Assets/SDF/Includes/matrix.cginc"
        #include "Assets/SDF/Includes/types.cginc"

        sampler2D _MainTex;
        float4 _MainTex_ST;
        float4 _TorusSizes;
        float _EPSILON_RAY;
        float _EPSILON_NORMAL;
        float _MAX_DISTANCE;
        float _MAX_STEPS;
        ENDHLSL

        Pass
        {
            HLSLPROGRAM
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.ro = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1));
                o.hitpos = v.vertex;
                return o;
            }

            Hit __SDF(float3 p)
            {
                // float4x4 rot = m_rotate(_Time.y, float3(0, 0, 1));
                // p = mul(rot, p);
                rZ(p, _Time.y/3);
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

            static fixed4 _COLORS[] = {
                fixed4(1, 0, 1, 1), // NO_ID
                // ------------------- VALID MATERIALS BELOW
                fixed4(0, 1, 0, .5),
            };
            
            fixed4 __MATERIAL(int id)
            {
                return _COLORS[id+1];
            }

            void castRay(inout RayInfo3D ray)
            {
                Hit hit = {_MAX_DISTANCE, NO_ID};
                ray.hit = hit;

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

            f2p frag(v2f i)
            {
                RayInfo3D ray = (RayInfo3D)0;

                ray.ro = i.ro;
                ray.rd = normalize(i.hitpos - i.ro);

                castRay(ray);

                // clip(ray.hit.id); // discard unhit rays

                float3 n = __SDF_NORMAL(ray.p);

                float near = _ProjectionParams.y;
                float far = _ProjectionParams.z;

                float zc = mul(float4( ray.p, 1.0 ), unity_CameraProjection).z;
                float wc = mul(float4( ray.p, 1.0 ), unity_CameraProjection ).w;
                float d = zc/wc;
                
                // GAMMA
                // col = pow(col, 0.45);
                fixed4 col = __MATERIAL(ray.hit.id);
                fixed3 n_col = normalize(n) * .5 + .5;
                
                f2p o = {
                    {fixed4(n_col, 1)},
                    {float3(normalize(n) * .5 + .5)},
                    (ID) ray.hit.id,
                    d,
                };
                return o;
            }
            // -----------------------------------
            ENDHLSL
        }
    }
}