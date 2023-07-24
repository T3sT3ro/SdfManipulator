using System;
using System.Linq;
using API;
using BuiltInTarget.Variables;

namespace BuiltInTarget {
    /// <summary>
    /// Implementation of builder for a HLSL+Shaderlab target.
    /// It is a root node of the graph
    /// </summary>
    [ShaderInclude(
        "Packages/SDF/Builders/BuiltInTarget/Includes/ShaderlabVariables.cs",
        "UnityCG.cginc",
        "Packages/SDF/Logic/Includes/types.cginc",
        "Packages/SDF/Logic/Includes/util.cginc",
        "Packages/SDF/Logic/Includes/matrix.cginc",
        "Packages/SDF/Logic/Includes/primitives.cginc",
        "Packages/SDF/Logic/Includes/operators.cginc",
        "Packages/SDF/Logic/Includes/noise.cginc"
    )]
    public static class UnlitShaderBuilder {

        public static readonly GraphBuilder.Evaluator evaluator = (includes, variables, vertIn, v2f_in, v2f_out, fragOut) =>
            $@"
Shader ""SDF/Domain""
{{
    // PROP BLOCK
    Properties
    {{
        [Header(Shader properties)][Space]
        //        [Toggle(_SCENEVIEW)] _SCENEVIEW (""Scene view"", Int) = 0
        //        [HideInInspector] _MainTex (""MainTex"", 2D) = ""white"" {{}} // used for image effect shader in sceneview
        [Enum(UnityEngine.Rendering.CullMode)] _Cull (""Cull"", Int) = 0
        [Tooltip(Enable to assure correct blending of multiple domains and backface rendering)]
        [Toggle][KeyEnum(Off, On)] _ZWrite (""ZWrite"", Float) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest(""ZTest"", Int) = 1

        [Header(Controls)][Space]
        {String.Join("\n       ", variables.Select(v => ShaderlabVariable.GetEvaluator(v).Invoke()))}

        [Space]
        [KeywordEnum(Material, Albedo, Texture, NormalLocal, NormalWorld, ID, Steps, Depth, Debug)] _DrawMode(""Draw mode"", Int) = 0
        [KeywordEnum(Near, Face)] _RayOrigin(""Ray origin"", Int) = 0
        [KeywordEnum(World, Local)] _Origin(""Scene origin"", Int) = 0
        [Tooltip(Only works for origin type local)]
        [Toggle] _PRESERVE_SPACE_SCALE (""preserve space scale"", Int) = 1
        _DomainOrigin (""domain origin"", Vector) = (0,0,0,0)
        _DomainRotation (""domain rotation"", Vector) = (0,0,0,0)

        [Header(SDF Scene)][Space]
        _Control (""size1, size2, rot1, rot2"", Vector) = (.5, .1, 0, 0)
        _BoxmapTex_X (""Texture for Triplanar mapping"", 2D) = ""white"" {{}}
        _BoxmapTex_Y (""Texture for Triplanar mapping"", 2D) = ""white"" {{}}
        _BoxmapTex_Z (""Texture for Triplanar mapping"", 2D) = ""white"" {{}}
        _DBG (""DBG"", Vector) = (0,0,0,0)
    }}

    //    Fallback ""Diffuse""

    SubShader
    {{
        Tags
        {{
            ""RenderType""=""Geometry""
            ""Queue""=""Geometry+1"" // +1 to resolve artifacts when using Cull Off
            ""IgnoreProjector""=""True""
        }}
        //        Blend SrcAlpha OneMinusSrcAlpha
        ZTest [_ZTest] // Can be customized manually, but when meshes intersect SDFs, this helps to draw both properly  
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
            _DRAWMODE_NORMALWORLD _DRAWMODE_ID _DRAWMODE_STEPS _DRAWMODE_DEPTH _DRAWMODE_DEBUG
        #pragma shader_feature_local _SCALE_INVARIANT
        #pragma shader_feature_local _ZWRITE_ON _ZWRITE_OFF
        #pragma shader_feature_local _PRESERVE_SPACE_SCALE_ON
        // #pragma shader_feature_local _SCENEVIEW

        {String.Join("\n        ", includes)}

        sampler2D _BoxmapTex_X;
        sampler2D _BoxmapTex_Y;
        sampler2D _BoxmapTex_Z;
        float4 _BoxmapTex_X_ST;
        float4 _BoxmapTex_Y_ST;
        float4 _BoxmapTex_Z_ST;

        UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);

        float4 _Control;

        {String.Join("\n        ", variables.Select(v => HlslVariable.GetEvaluator(v).Invoke()))}

        // NON-PROEPRTY UNIFORMS
        float4x4 _BoxFrame1_Transform = MATRIX_ID;
        float4x4 _Sphere1_Transform = MATRIX_ID;

        float4 _DBG;
        float4 _DomainOrigin;
        float4 _DomainRotation;


        // calculated stuff

        // Domain -> Model -> World -> Projection
        static const float4x4 MATRIX_DOMAIN = m_translate(_DomainOrigin);
        static const float4x4 MATRIX_I_DOMAIN = m_translate(-_DomainOrigin);
        
        static const float4x4 SCALE_MATRIX =
            #if defined(_PRESERVE_SPACE_SCALE_ON) && defined(_ORIGIN_LOCAL)
            extract_scale_matrix(UNITY_MATRIX_M);
            #else
            MATRIX_ID;
        #endif

        // inverse 
        static const float4x4 SCALE_MATRIX_I = inverse_diagonal(SCALE_MATRIX); // used for non-uniform scale

        // inverse projection matrix either to world or to model, depending on the origin type
        // instead of performing matrix inversion in the shader, use already supplied matrices
        static const float4x4 inv = mul(SCALE_MATRIX,
                                        #ifdef _ORIGIN_WORLD
                                            mul(UNITY_MATRIX_I_V, unity_CameraInvProjection)
                                            // inverse(UNITY_MATRIX_VP)
                                        #else
                                        mul(unity_WorldToObject, mul(UNITY_MATRIX_I_V, unity_CameraInvProjection))
                                        // inverse(UNITY_MATRIX_MVP)
                                        #endif
        );

        //const float near = _ProjectionParams.y; // those go into frag
        //const float far = _ProjectionParams.z;
        // https://gist.github.com/unitycoder/c5847a82343a8e721035
        // static const float3 camera_forward = UNITY_MATRIX_IT_MV[2].xyz;
        ENDHLSL

        // DEPTH PREPASS - limits rays going beyond backface of shader
        //        Pass {{
        //            Cull Front
        //            ZWrite On
        //            ColorMask 0
        //        }}

        ShaderData.Pass
        {{
            HLSLPROGRAM
            v2f vert(appdata_base v)
            {{
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex); // clip space
                o.screenPos = ComputeScreenPos(o.vertex); // from 0,0 to 1,1
                // o.uv = v.texcoord; // TRANSFORM_TEX(v.texcoord, _BoxmapTex);
                o.hitpos = v.vertex;
                COMPUTE_EYEDEPTH(o.screenPos.z);
                return o;
            }}

            /*
            Hit __SDF_BoxFrame1(float3 p)   
            {{
                Hit hit = (Hit)0;

                //transform space
                p = mul(p, _BoxFrame1_Transform);

                hit.distance = sdf::primitives3D::box_frame(p, _Control.x, _Control.y);
                hit.id = 1;

                Material mat = (Material)0;
                mat.Albedo = fixed4(0, .7, .2, .5); // grass :)
                mat.Emission = 0;
                hit.material = mat;
                return hit;
            }}

            Hit __SDF_Repeat_Space(float3 p)
            {{
                // transform point
                rotX(p, _Control.z); //_Time.y / 3);
                rotY(p, _Control.w);

                // repeat operator 
                int3 ix;
                p = sdf::operators::repeatLim(p, 1, float3(1, 0, 1), ix);

                // primitive 
                Hit hit = __SDF_BoxFrame1(p);

                // material
                Material mat = (Material)0;
                static fixed4 COLORS[] = {{
                    fixed4(0, .7, .2, .5), // grass :)
                    fixed4(1, .7, .2, .5), // orange :)
                    fixed4(1, 0, .2, .5) // tomato :)
                }};
                mat.Albedo = COLORS[ix.x + 1];
                hit.material = mat;

                return hit;
            }}

            Hit __SDF_Sphere1(float3 p)
            {{
                p = mul(p, _Sphere1_Transform);
                Hit hit = (Hit)0;
                hit.distance = sdf::primitives3D::sphere(p, _Control.z);
                hit.id = 2;
            
                return hit;
            }}

            // smooth min with blending factor from IQ: https://iquilezles.org/articles/smin/
            float2 sminN(float d1, float d2, float k, float n, out float t)
            {{
                float h = max(k - abs(d1 - d2), 0.0) / k;
                t = pow(h, n) * 0.5;
                float s = t * k / n;
                if (d1 < d2) return d1 - s;
                t = 1.0 - t;
                return d2 - s;
            }}

            Material blendMaterials(Material a, Material b, float t)
            {{
                Material res;
                res.Albedo = lerp(a.Albedo, b.Albedo, t);
                res.Emission = lerp(a.Emission, b.Emission, t);
                res.Metallic = lerp(a.Metallic, b.Metallic, t);
                // TODO: move it elsewehere
                res.Normal = normalize(lerp(a.Normal, b.Normal, t)); // this actually would be better off as another OP
                res.Occlusion = lerp(a.Occlusion, b.Occlusion, t);
                res.Smoothness = lerp(a.Smoothness, b.Smoothness, t);
                return res;
            }}

            Hit __SDF_Smoothmax_BoxFrame1_Sphere1(float3 p)
            {{
                Hit boxframe1 = __SDF_BoxFrame1(p);
                Hit sphere1 = __SDF_Sphere1(p);
                Hit hit;
                float t;
                hit.distance = sminN(boxframe1.distance, sphere1.distance, 0.0, 1.0, t);
                hit.id = lerp(boxframe1.id, sphere1.id, step(0.5, t));

                hit.material = blendMaterials(boxframe1.material, sphere1.material, t);
                return hit;
            }}

            Hit __SDF(float3 p)
            {{
                return __SDF_Smoothmax_BoxFrame1_Sphere1(p);
            }}
            
            #1#
            // =======================================================================

            SdfResult __SDF(float3 p)
            {{
                // float4x4 rot = m_rotate(_Time.y, float3(0, 0, 1));
                // p = mul(rot, p);
                rotX(p, _Control.z); //_Time.y / 3);
                rotY(p, _Control.w);
                // Hit ret = {{sdf::primitives3D::torus(p, _TorusSizes.x, _TorusSizes.y), 0}};
                int3 ix;
                p = sdf::operators::repeatLim(p, 1, float3(1, 0, 1), ix);
                SdfResult ret;
                ret.distance = sdf::primitives3D::box_frame(p, _Control.x, _Control.y);
                ret.id.xyz = ix + 1;
                ret.id.w = 0;
                return ret;
            }}

            // https://iquilezles.org/www/articles/normalsSDF/normalsSDF.htm
            float3 __SDF_NORMAL(float3 p)
            {{
                // using tetrahedron technique
                // EPSILON -- can be adjusted using pixel footprint
                const float2 k = float2(1, -1);
                return normalize(
                    k.xyy * __SDF(p + k.xyy * _EPSILON_NORMAL).distance +
                    k.yyx * __SDF(p + k.yyx * _EPSILON_NORMAL).distance +
                    k.yxy * __SDF(p + k.yxy * _EPSILON_NORMAL).distance +
                    k.xxx * __SDF(p + k.xxx * _EPSILON_NORMAL).distance
                );
            }}


            // TODO: lighting
            fixed4 __MATERIAL(fixed4 id)
            {{
                static fixed4 _COLORS[] = {{
                    fixed4(.5, 0, .5, 1), // NO_ID (magenta)
                    // ------------------- VALID MATERIALS BELOW
                    fixed4(0, .7, .2, .5), // grass :)
                    fixed4(1, .7, .2, .5), // orange :)
                    fixed4(1, 0, .2, .5), // tomato :)
                }};
                return _COLORS[id.x + 1];
            }}

            // triplanar mapping texture, point, normal, smoothing
            fixed4 __BOXMAP(BoxMapParams3D params, in float3 p, in float3 n, in float k)
            {{
                fixed4 x = tex2D(params.X, p.zy * params.X_ST.xy + params.X_ST.zw);
                fixed4 y = tex2D(params.Y, p.zx * params.Y_ST.xy + params.Y_ST.zw);
                fixed4 z = tex2D(params.Z, p.xy * params.Z_ST.xy + params.Z_ST.zw);

                float3 w = pow(abs(n), k);

                return (x * w.x + y * w.y + z * w.z) / (w.x + w.y + w.z);
            }}

            // -----------------------------------------------------------------------

            // returns sdf and ray point
            void castRay(inout SdfResult sdf, inout Ray3D ray)
            {{
                float d = ray.startDistance;
                sdf.distance = _MAX_DISTANCE;
                sdf.id = int4(NO_ID);

                sdf.material = (Material)0;
                sdf.normal = 0;

                for (ray.steps = 0; ray.steps < _MAX_STEPS; ray.steps++)
                {{
                    if (d >= _MAX_DISTANCE || d >= ray.maxDistance)
                        return;

                    sdf.p = ray.ro + d * ray.rd;

                    SdfResult sdfOnRay = __SDF(sdf.p);
                    if (sdfOnRay.distance < _EPSILON_RAY)
                    {{
                        sdf.distance = d;
                        sdf.id = sdfOnRay.id;
                        return;
                    }}

                    d += sdfOnRay.distance;
                }}
            }}

            // =======================================================================

            float depthToMaxRayDepth(in float2 screenUV, in float3 rd)
            {{
                // read camera depth texture to correctly blend with scene geometry
                // beware, that _CameraDepthTexture IS NOT the depth buffer!
                // it is populated in the prepass and doesn't change in subsequent passes
                // https://forum.unity.com/threads/does-depth-buffer-update-between-passes.620575/
                float camDepth = CorrectDepth(tex2D(_CameraDepthTexture, screenUV).rg);

                float4 forward = mul(inv, float4(0, 0, 1, 1)); // ray end on far plane
                forward /= forward.w;
                forward = normalize(forward); // forward in object space
                return camDepth / dot(forward, rd);
            }}

            Ray3D getRaysForCamera(float3 screenPos, float3 objectHitpos)
            {{
                Ray3D ray = (Ray3D)0;

                // NDC from (-1, -1, -1) to (1, 1, 1) 
                float3 NDC = 2. * screenPos.xyz - 1.;

                float4 ro = mul(inv, float4(NDC.xy, UNITY_NEAR_CLIP_VALUE, 1)); // ray origin on near plane
                ro /= ro.w;
                #ifndef _RAYORIGIN_NEAR
                {{
                    float4 rs =
                        #ifdef _ORIGIN_WORLD
                            mul(UNITY_MATRIX_M, float4(objectHitpos, 1));
                        #else
                        fixed4(objectHitpos, 1);
                    #endif
                    ray.startDistance = distance(mul(rs, SCALE_MATRIX), ro); // start on ray
                }}
                #endif

                float4 re = mul(inv, float4(NDC.xy, 1, 1)); // ray end on far plane
                re /= re.w;
                float3 rd = normalize((re - ro).xyz); // ray direction

                ray.ro = ro;
                ray.rd = rd;
                ray.startDistance += _RAY_ORIGIN_BIAS;
                ray.maxDistance = depthToMaxRayDepth(screenPos.xy, ray.rd);
                return ray;
            }}

            f2p frag(v2f i, fixed facing : VFACE)
            {{
                const float3 screenPos = i.screenPos.xyz / i.screenPos.w; // 0,0 to 1,1 on screen

                Ray3D ray = getRaysForCamera(screenPos, i.hitpos);
                SdfResult sdf = (SdfResult)0;

                castRay(sdf, ray);

                clip(sdf.id.w); // discard rays without hit

                sdf.normal = __SDF_NORMAL(sdf.p);

                fixed4 color_material = __MATERIAL(sdf.id.x); // color
                BoxMapParams3D boxmapParams = {{
                    _BoxmapTex_X, _BoxmapTex_Y, _BoxmapTex_Z, {{_BoxmapTex_X_ST}}, {{_BoxmapTex_Y_ST}}, {{_BoxmapTex_Z_ST}}
                }};
                fixed4 color_trimap = __BOXMAP(boxmapParams, sdf.p, sdf.normal, 10.);

                fixed4 color_normal_domain = fixed4(sdf.normal * .5 + .5, 1); // domain normal color
                fixed4 color_normal_world = fixed4(UnityObjectToWorldNormal(sdf.normal) * .5 + .5, 1);
                // world normal color

                fixed4 color_id = sdf.id;

                float eyeDepth =
                    -
                    #ifdef _ORIGIN_WORLD
                    UnityWorldToViewPos
                    #else
                    UnityObjectToViewPos
                    #endif
                    (mul(SCALE_MATRIX_I, sdf.p)).z;

                f2p o = {{
                    {{
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
                         fixed4((float3)eyeDepth/5., 1)
                        #elif _DRAWMODE_DEBUG
                         fixed4(i.vertex.xyz, 1)
                        #endif
                    }},
                    {{float3(normalize(sdf.normal) * .5 + .5)}},
                    {{sdf.id}},
                    #ifdef _ZWRITE_ON
                    EncodeCorrectDepth(eyeDepth)
                    #endif
                }};
                return o;
            }} // End Pass

            // =======================================================================
            ENDHLSL
        }} // END PASS
    }}
}}
";
    }
}
