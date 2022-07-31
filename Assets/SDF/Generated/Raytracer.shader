Shader "SDF/Domain"
{
    // PROP BLOCK
    Properties
    {
        _MainTex ("Triplanar", 2D) = "white" {}
        _TorusSizes ("Torus R, r, rot, mat", Vector) = (.5, .1, 0, 0)
        _MAX_STEPS ("max steps of raymarcher", Int) = 200
        _MAX_DISTANCE ("max distance a ray can march", Float) = 200.0
        _START_BIAS ("ray start offset from the ray's origin", Float) = 0.01
        _EPSILON_RAY ("minimum distance for ray to consider the surface hit", Float) = 0.001
        _EPSILON_NORMAL ("epsilon for claculating normal", Float) = 0.001
        _DBG ("x - step value", Vector) = (0,0,0,0)
    }

    //    Fallback "Diffuse"

    SubShader
    {
        Tags
        {
            "RenderType"="Geometry"
            "Queue"="Geometry+1" // +1 to resolve artifacts when using Cull Off
            "IgnoreProjector"="True"
        }
        //        Blend SrcAlpha OneMinusSrcAlpha
        ZTest Off // When
        Cull Off // When camera is inside domain

        // common includes for all passes
        HLSLINCLUDE
        #pragma target 5.0
        #pragma vertex vert
        #pragma fragment frag

        #include "UnityCG.cginc"
        #include "Assets/SDF/Includes/primitives.cginc"
        #include "Assets/SDF/Includes/operators.cginc"
        #include "Assets/SDF/Includes/noise.cginc"
        #include "Assets/SDF/Includes/types.cginc"
        #include "Assets/SDF/Includes/matrix.cginc" // FIXME: inverse might be a problem

        sampler2D _MainTex;
        float4 _MainTex_ST;
        int _RAND_INSTANCE_COL;

        float4 _TorusSizes;
        float _EPSILON_RAY;
        float _EPSILON_NORMAL;
        float _MAX_DISTANCE;
        float _START_BIAS;
        float _MAX_STEPS;
        int _WORLD_SPACE;

        float4 _DBG;

        UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);

        // #define WORLDSPACE
        #define START_AT_FACE


        // https://gist.github.com/unitycoder/c5847a82343a8e721035
        // static const float3 camera_forward = UNITY_MATRIX_IT_MV[2].xyz;
        static const float4x4 inv = inverse(
            #ifdef WORLDSPACE
            UNITY_MATRIX_VP
            #else
            UNITY_MATRIX_MVP
            #endif
        );
        //const float near = _ProjectionParams.y; // those go into frag
        //const float far = _ProjectionParams.z;
        ENDHLSL

        Pass
        {
            HLSLPROGRAM
            v2f vert(appdata_base v, uint id: SV_VertexID)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex); // clip space
                o.screenPos = ComputeScreenPos(o.vertex); // UV on from 0,0 to 1,1
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.hitpos = v.vertex; 
                COMPUTE_EYEDEPTH(o.screenPos.z);
                return o;
            }

            // =======================================================================
            Hit __SDF(float3 p)
            {
                // float4x4 rot = m_rotate(_Time.y, float3(0, 0, 1));
                // p = mul(rot, p);
                // rotZ(p, _TorusSizes.z); //_Time.y / 3);
                // Hit ret = {sdf::primitives3D::torus(p, _TorusSizes.x, _TorusSizes.y), 0};
                int3 ix;
                p = sdf::operators::repeatLim(p, 1, float3(1, 0, 1), ix);
                Hit ret = {
                    sdf::primitives3D::sphere(
                        p
                        , _TorusSizes.x
                        // , _TorusSizes.y
                    ),
                    ix.x + 1 // random number for instance
                };
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


            // TODO: lighting
            fixed4 __MATERIAL(int id)
            {
                static fixed4 _COLORS[] = {
                    fixed4(.5, 0, .5, 1), // NO_ID (magenta)
                    // ------------------- VALID MATERIALS BELOW
                    fixed4(0, .7, .2, .5), // grass :)
                    fixed4(1, .7, .2, .5), // orange :)
                    fixed4(1, 0, .2, .5), // tomato :)
                };
                return _COLORS[id + 1];
            }

            // triplanar mapping texture, point, normal, smoothing
            fixed4 __BOXMAP(sampler2D tex, in float4 tex_ST, in float3 p, in float3 n, in float k)
            {
                fixed4 x = tex2D(tex, p.zy * tex_ST.xy + tex_ST.zw);
                fixed4 y = tex2D(tex, p.zx * tex_ST.xy + tex_ST.zw);
                fixed4 z = tex2D(tex, p.xy * tex_ST.xy + tex_ST.zw);

                float3 w = pow(abs(n), k);

                return (x * w.x + y * w.y + z * w.z) / (w.x + w.y + w.z);
            }

            // -----------------------------------------------------------------------

            void castRay(inout RayInfo3D ray, in float max_distance)
            {
                float d = ray.hit.distance;
                Hit hit = {_MAX_DISTANCE, NO_ID};
                ray.hit = hit;
                for (ray.steps = 0; ray.steps < _MAX_STEPS; ray.steps++)
                {
                    ray.p = ray.ro + d * ray.rd;

                    Hit hit = __SDF(ray.p);
                    if (hit.distance < _EPSILON_RAY)
                    {
                        ray.hit.distance = d;
                        ray.hit.id = hit.id;
                        return;
                    }

                    d += hit.distance;

                    if (d >= _MAX_DISTANCE || d >= max_distance)
                        return;
                }
            }

            // returns correct depth in both persp and ortho
            // from https://forum.unity.com/threads/depth-buffer-with-orthographic-camera.355878/
            // from https://forum.unity.com/threads/getting-scene-depth-z-buffer-of-the-orthographic-camera.601825/#post-4966334
            
            float CorrectDepth(float rawDepth)
            {
                float persp = LinearEyeDepth(rawDepth);
                float ortho = (_ProjectionParams.z - _ProjectionParams.y) * rawDepth + _ProjectionParams.y;
                return lerp(persp, ortho, unity_OrthoParams.w);
            }

            // =======================================================================

            f2p frag(v2f i)
            {
                const float3 screenPos = i.screenPos.xyz / i.screenPos.w; // 0,0 to 1,1 on screen

                RayInfo3D ray = (RayInfo3D)0;

                // NDC from (-1, -1, -1) to (1, 1, 1) 
                float2 NDC = 2. * screenPos.xy - 1.;

                float4 ro = mul(inv, float4(NDC.xy, UNITY_NEAR_CLIP_VALUE, 1)); // ray origin on near plane
                ro /= ro.w;

                float4 re = mul(inv, float4(NDC.xy, 1, 1)); // ray end on far plane
                re /= re.w;
                float3 rd = normalize((re - ro).xyz); // ray direction

                ray.ro = ro; // in object space
                ray.rd = rd; // in object space

                #ifdef START_AT_FACE
                    ray.hit.distance = length(i.hitpos - ro) + _START_BIAS;
                #endif

                // read camera depth texture to correctly blend with scene geometry
                float depth = CorrectDepth(tex2D(_CameraDepthTexture, screenPos.xy).r);

                float4 forward = mul(inv, float4(0, 0, 1, 1)); // ray end on far plane
                forward = normalize(forward / forward.w); // forward in object space

                float3 dots = dot(forward, rd);
                fixed4 color_dot = fixed4(pow(dots.xyz, 10), 1.0);

                castRay(ray, depth / dots);


                clip(ray.hit.id); // discard rays without hit

                ray.n = __SDF_NORMAL(ray.p);

                // GAMMA
                // col = pow(col, 0.45);
                fixed4 color_material = __MATERIAL(ray.hit.id); // color
                fixed4 color_trimap = __BOXMAP(_MainTex, _MainTex_ST, ray.p, ray.n, 10.);

                fixed4 color_normal_domain = fixed4(ray.n * .5 + .5, 1); // domain normal color
                fixed4 color_normal_world = fixed4(UnityObjectToWorldNormal(ray.n) * .5 + .5, 1); // world normal color


                // not working
                fixed4 colors[] = {
                    {color_material},
                    {color_normal_domain},
                    {color_normal_world},
                    {color_dot},
                    {color_trimap}
                };


                f2p o = {
                    {LEVELS(colors, _TorusSizes.w)},
                    {float3(normalize(ray.n) * .5 + .5)},
                    ray.hit.id,
                };
                return o;
            }

            // =======================================================================
            ENDHLSL
        }
    }
}