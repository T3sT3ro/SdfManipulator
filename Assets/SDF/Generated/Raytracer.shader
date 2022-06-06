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
        [Header(Debug)]
        _DBG ("x:ro, y:rd, z:norm, w:ro+rd", Vector) = (0,0,0,0)
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
        #include "HLSLSupport.cginc"
        #include "Assets/SDF/Includes/primitives.cginc"
        #include "Assets/SDF/Includes/types.cginc"
        #include "Assets/SDF/Includes/matrix.cginc" // FIXME: inverse might be a problem

        sampler2D _MainTex;
        float4 _MainTex_ST;
        float4 _TorusSizes;
        float4 _DBG;
        float _EPSILON_RAY;
        float _EPSILON_NORMAL;
        float _MAX_DISTANCE;
        float _MAX_STEPS;
        ENDHLSL

        Pass
        {
            HLSLPROGRAM
            v2f vert(appdata_base v, uint id: SV_VertexID)
            {
                v2f o;
                
                o.vertex = UnityObjectToClipPos(v.vertex); // clip space
                //
                // if(id == 0)
                // {
                //     o.vertex.xyzw = float4(-1, -1, 0, 1); 
                //     
                // }
                // // perspective OK
                o.p_ok_ro = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1));
                o.p_ok_hit = v.vertex;
                //
                // // ortho OK
                float4x4 INV = inverse(UNITY_MATRIX_MVP); // to world space or UNITY_MATRIX_VP, unity_MatrixMVP 
                o.o_ok_ro = mul(INV, float4(o.vertex.xy/o.vertex.w, -1, 1)); // ray start on frustum
                o.o_ok_re = mul(INV, float4(o.vertex.xy/o.vertex.w, 1, 1)); // ray end on frustum
                //
                return o;
            }

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

            float4x4 PVI;
            
            f2p frag(v2f i)
            {
                RayInfo3D ray = (RayInfo3D)0;
                Hit hit = {_MAX_DISTANCE, NO_ID};
                ray.hit = hit;
                
                // perspective OK -- first implementation
                    float3 p_ok_ro = i.p_ok_ro;
                    float3 p_ok_rd = normalize(i.p_ok_hit - i.p_ok_ro);
                //

                // ortho OK -- second, cast from perspective matrix
                    float3 o_ok_ro = i.o_ok_ro;
                    float3 o_ok_rd = normalize(i.o_ok_re - i.o_ok_ro);    
                //

                // fragment based ray calculation
                    float4x4 inv = inverse(UNITY_MATRIX_MVP); //UNITY_MATRIX_VP 
                    float3 f_ro = mul(inv, float4(i.vertex.xy/_ScreenParams.xy, UNITY_NEAR_CLIP_VALUE, 1)); // ray start on frustum
                    float3 f_re = mul(inv, float4(i.vertex.xy/_ScreenParams.xy, 1, 1)); // ray end on frustum
                    float3 f_rd = normalize(f_re-f_ro);

                // WIP
                    float4 wip_ro = mul(inv, float4(i.vertex.xy/_ScreenParams.xy, UNITY_NEAR_CLIP_VALUE, 1));
                    wip_ro/=wip_ro.w;

                    float4 wip_re = mul(inv, float4(i.vertex.xy/_ScreenParams.xy, 1, 1));
                    wip_re /= wip_re.w;
                    float3 wip_rd = normalize(wip_re.xyz - wip_ro.xyz);
                //

                // multi-lerp code
                float3 ros[] = {{p_ok_ro}, {o_ok_ro}, {f_ro}, {wip_ro.xyz}};
                float3 rds[] = {{p_ok_rd}, {o_ok_rd}, {f_rd}, {wip_rd.xyz}};
                
                ray.ro = lerp(ros[floor(_DBG.x+_DBG.w)], ros[ceil(_DBG.x+_DBG.w)], frac(_DBG.x+_DBG.w)); 
                ray.rd = lerp(rds[floor(_DBG.y+_DBG.w)], rds[ceil(_DBG.y+_DBG.w)], frac(_DBG.y+_DBG.w));
                //
                    
                castRay(ray);

                // clip(ray.hit.id); // discard no-hit rays

                float3 n = __SDF_NORMAL(ray.p);

                // GAMMA
                // col = pow(col, 0.45);
                fixed4 mat_col = __MATERIAL(ray.hit.id); // color
                fixed4 n_col = fixed4(normalize(n) * .5 + .5, 1); // domain normal color
                fixed4 clip_col = fixed4(i.vertex.xy/_ScreenParams.xy, i.vertex.z, 1); // RG - clip pos of vertex 
                fixed4 dir_col = fixed4(ray.rd,1);
            
                fixed4 colors[] = {{mat_col}, {n_col}, {clip_col}, {dir_col}};
                // n_col = fixed4(i.vertex.xyz, 1);
                fixed4 col = lerp(colors[floor(_DBG.z)], colors[ceil(_DBG.z)], frac(_DBG.z)); 


                f2p o = {
                    {col},
                    {float3(normalize(n) * .5 + .5)},
                    ray.hit.id,
                };
                return o;
            }

            // -----------------------------------
            ENDHLSL
        }
    }
}