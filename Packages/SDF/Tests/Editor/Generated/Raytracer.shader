Shader "SDF/Domain"
{
    // PROP BLOCK
    Properties
    {
        [Header(Shader properties)][Space]
        //        [Toggle(_SCENEVIEW)] _SCENEVIEW ("Scene view", Int) = 0
        //        [HideInInspector] _MainTex ("MainTex", 2D) = "white" {} // used for image effect shader in sceneview
        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull", Int) = 0
        [Tooltip(Enable to assure correct blending of multiple domains and backface rendering)]
        [Toggle][KeyEnum(Off, On)] _ZWrite ("ZWrite", Float) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Int) = 1

        [Header(Raymarcher)][Space]
        _MAX_STEPS ("max raymarching steps", Int) = 200
        _MAX_DISTANCE ("marx raymarching distance", Float) = 200.0
        _RAY_ORIGIN_BIAS ("ray origin bias", Float) = 0
        _EPSILON_RAY ("epsilon step for ray to consider hit", Float) = 0.001
        _EPSILON_NORMAL ("epsilon for calculating normal", Float) = 0.001

        [Space]
        [KeywordEnum(Material, Albedo, Texture, NormalLocal, NormalWorld, ID, Steps, Depth)] _DrawMode("Draw mode", Int) = 0
        [KeywordEnum(Near, Face)] _RayOrigin("Ray origin", Int) = 0
        [KeywordEnum(World, Local)] _Origin("Scene origin", Int) = 0
        [Tooltip(Only works for origin type local)]
        [Toggle] _PRESERVE_SCALE ("Preserve local scale", Int) = 1

        [Header(SDF Scene)][Space]
        _Control ("size1, size2, rot1, rot2", Vector) = (.5, .1, 0, 0)
        _BoxmapTex_X ("Texture for Triplanar mapping", 2D) = "white" {}
        _BoxmapTex_Y ("Texture for Triplanar mapping", 2D) = "white" {}
        _BoxmapTex_Z ("Texture for Triplanar mapping", 2D) = "white" {}
        _DBG ("DBG", Vector) = (0,0,0,0)
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
        ZTest LEqual // Can be customized manually, but when meshes intersect SDFs, this helps to draw both properly  
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
        #pragma shader_feature_local _PRESERVE_SCALE_ON
        // #pragma shader_feature_local _SCENEVIEW

        #include "UnityCG.cginc"
        #include "Packages/SDF/Editor/Includes/primitives.cginc"
        #include "Packages/SDF/Editor/Includes/operators.cginc"
        #include "Packages/SDF/Editor/Includes/noise.cginc"
        #include "Packages/SDF/Editor/Includes/types.cginc"
        #include "Packages/SDF/Editor/Includes/util.cginc"
        #include "Packages/SDF/Editor/Includes/matrix.cginc"

        static const float4x4 SCALE_MATRIX =
            #if defined(_PRESERVE_SCALE_ON) && defined(_ORIGIN_LOCAL)
            {
                {length(float3(UNITY_MATRIX_M[0].x, UNITY_MATRIX_M[1].x, UNITY_MATRIX_M[2].x)), 0, 0, 0},
                {0, length(float3(UNITY_MATRIX_M[0].y, UNITY_MATRIX_M[1].y, UNITY_MATRIX_M[2].y)), 0, 0},
                {0, 0, length(float3(UNITY_MATRIX_M[0].z, UNITY_MATRIX_M[1].z, UNITY_MATRIX_M[2].z)), 0},
                {0, 0, 0, 1}
            };
            #else
            MATRIX_ID;
        #endif

        static const float4x4 SCALE_MATRIX_I = inverse(SCALE_MATRIX); // used for non-uniform scale

        // https://gist.github.com/unitycoder/c5847a82343a8e721035
        // static const float3 camera_forward = UNITY_MATRIX_IT_MV[2].xyz;
        static const float4x4 inv = mul(SCALE_MATRIX, inverse(
                                            #ifdef _ORIGIN_WORLD
                                            UNITY_MATRIX_VP
                                            #else
                                            UNITY_MATRIX_MVP
                                            #endif
                                        ));

        sampler2D _BoxmapTex_X;
        sampler2D _BoxmapTex_Y;
        sampler2D _BoxmapTex_Z;
        float4 _BoxmapTex_X_ST;
        float4 _BoxmapTex_Y_ST;
        float4 _BoxmapTex_Z_ST;

        UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);

        float4 _Control;
        float _EPSILON_RAY;
        float _EPSILON_NORMAL;
        float _MAX_DISTANCE;
        float _RAY_ORIGIN_BIAS;
        float _MAX_STEPS;

        float4 _DBG;


        //const float near = _ProjectionParams.y; // those go into frag
        //const float far = _ProjectionParams.z;
        ENDHLSL

// DEPTH PREPASS - limits rays going beyond backface of shdaer
//        Pass {
//            Cull Front
//            ZWrite On
//            ColorMask 0
//        }
        
        Pass
        {
            HLSLPROGRAM
            v2f vert(appdata_base v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex); // clip space
                o.screenPos = ComputeScreenPos(o.vertex); // from 0,0 to 1,1
                // o.uv = v.texcoord; // TRANSFORM_TEX(v.texcoord, _BoxmapTex);
                o.hitpos = v.vertex;
                COMPUTE_EYEDEPTH(o.screenPos.z);
                return o;
            }

            // =======================================================================
            Hit __SDF(float3 p)
            {
                // float4x4 rot = m_rotate(_Time.y, float3(0, 0, 1));
                // p = mul(rot, p);
                rotX(p, _Control.z); //_Time.y / 3);
                rotY(p, _Control.w);
                // Hit ret = {sdf::primitives3D::torus(p, _TorusSizes.x, _TorusSizes.y), 0};
                int3 ix;
                p = sdf::operators::repeatLim(p, 1, float3(1, 0, 1), ix);
                Hit ret = {
                    sdf::primitives3D::box_frame(
                        p
                        , _Control.x
                        , _Control.y
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
            fixed4 __BOXMAP(BoxMapParams3D params, in float3 p, in float3 n, in float k)
            {
                fixed4 x = tex2D(params.X, p.zy * params.X_ST.xy + params.X_ST.zw);
                fixed4 y = tex2D(params.Y, p.zx * params.Y_ST.xy + params.Y_ST.zw);
                fixed4 z = tex2D(params.Z, p.xy * params.Z_ST.xy + params.Z_ST.zw);

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
                    if (d >= _MAX_DISTANCE || d >= max_distance)
                        return;

                    ray.p = ray.ro + d * ray.rd;

                    Hit hit = __SDF(ray.p);
                    if (hit.distance < _EPSILON_RAY)
                    {
                        ray.hit.distance = d;
                        ray.hit.id = hit.id;
                        return;
                    }

                    d += hit.distance;
                }
            }

            // =======================================================================

            f2p frag(v2f i, fixed facing : VFACE)
            {
                const float3 screenPos = i.screenPos.xyz / i.screenPos.w; // 0,0 to 1,1 on screen

                RayInfo3D ray = (RayInfo3D)0;

                // NDC from (-1, -1, -1) to (1, 1, 1) 
                float3 NDC = 2. * screenPos.xyz - 1.;

                float4 ro = mul(inv, float4(NDC.xy, UNITY_NEAR_CLIP_VALUE, 1)); // ray origin on near plane
                ro /= ro.w;
                #ifndef _RAYORIGIN_NEAR
                {
                    float4 rs =
                        #ifdef _ORIGIN_WORLD
                        mul(UNITY_MATRIX_M, float4(i.hitpos, 1));
                        #else
                        fixed4(i.hitpos, 1);
                    #endif
                    ray.hit.distance = distance(mul(rs, SCALE_MATRIX), ro); // start on ray
                }
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

                ray.n = __SDF_NORMAL(ray.p);

                // GAMMA
                // col = pow(col, 0.45);
                fixed4 color_material = __MATERIAL(ray.hit.id); // color
                BoxMapParams3D boxmapParams = {_BoxmapTex_X, _BoxmapTex_Y, _BoxmapTex_Z, {_BoxmapTex_X_ST}, {_BoxmapTex_Y_ST}, {_BoxmapTex_Z_ST}};
                fixed4 color_trimap = __BOXMAP(boxmapParams, ray.p, ray.n, 10.);

                fixed4 color_normal_domain = fixed4(ray.n * .5 + .5, 1); // domain normal color
                fixed4 color_normal_world = fixed4(UnityObjectToWorldNormal(ray.n) * .5 + .5, 1); // world normal color


                fixed4 color_id = ray.hit.id;

                float eyeDepth = -UnityObjectToViewPos(mul(SCALE_MATRIX_I, ray.p)).z;

                f2p o = {
                    {
                        #ifdef _DRAWMODE_MATERIAL
                         color_material*color_trimap
                        #elif _DRAWMODE_ALBEDO
                         color_material
                        #elif _DRAWMODE_TEXTURE
                         color_trimap
                        #elif _DRAWMODE_NORMALLOCAL
                         color_normal_domain
                        #elif _DRAWMODE_NORMALWORLD
                         color_normal_world
                        #elif _DRAWMODE_ID
                         color_id
                        #elif _DRAWMODE_STEPS
                         lerp(fixed4(0,0,1,1), fixed4(1,0,0,1), ray.steps/_MAX_STEPS)
                        #elif _DRAWMODE_DEPTH
                         fixed4((float3)eyeDepth/4., 1)
                        #endif
                    },
                    {float3(normalize(ray.n) * .5 + .5)},
                    ray.hit.id,
                    #ifdef _ZWRITE_ON
                    EncodeCorrectDepth(eyeDepth)
                    #endif
                };
                return o;
            } // End Pass

            // =======================================================================
            ENDHLSL
        } // END PASS
    }
}