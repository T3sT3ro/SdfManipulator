Shader "Complete Sphere Impostor"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        [NoScaleOffset] _MainTex ("Equirectangular Texture", 2D) = "white" {}
        [NoScaleOffset] _CubeTex ("Cubemap Texture", Cube) = "white" {}
        [KeywordEnum(Equirectangular, Cubemap)] _Mapping ("UV Mapping", Float) = 0.0
        [KeywordEnum(None, AlphaToCoverage, SuperSample)] _MSAABehaviour ("MSAA Anti Aliasing", Float) = 0.0
    }
    SubShader
    {
        Tags { "Queue"="AlphaTest" "RenderType"="RaytracedSphere" "DisableBatching"="True" }
        LOD 100

        // not needed for rendering, you only ever see the front of the quad
        // but this makes Unity's scene selection allow for back face selection
        Cull Off

        CGINCLUDE
            // should make shadow receiving work on mobile
            #if defined(UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS)
            #undef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
            #endif

            #include "UnityCG.cginc"
            #include "AutoLight.cginc"

            // enable per subsample rendering
            // requires Shader Model 5.0
            #if defined(_MSAABEHAVIOUR_SUPERSAMPLE) && SHADER_TARGET > 40 && !defined(SHADER_TARGET_GLSL) && !defined(UNITY_PASS_SHADOWCASTER)
            #define MSAA_SUPER_SAMPLE 1
            #define PER_SAMPLE_INTERPOLATION sample
            #else
            #define MSAA_SUPER_SAMPLE 0
            #define PER_SAMPLE_INTERPOLATION
            #endif

            // real check needed for enabling conservative depth
            // requires Shader Model 5.0
            #if SHADER_TARGET > 40 && UNITY_REVERSED_Z
            #define USE_CONSERVATIVE_DEPTH 1
            #endif

            struct appdata
            {
                float4 vertex : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                PER_SAMPLE_INTERPOLATION float3 rayDir : TEXCOORD0;
                PER_SAMPLE_INTERPOLATION float3 rayOrigin : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            // common vertex function for all passes
            v2f vert (appdata v)
            {
                v2f o;

                // instancing
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                // check if the current projection is orthographic or not from the current projection matrix
                bool isOrtho = UNITY_MATRIX_P._m33 == 1.0;

                // viewer position, equivalent to _WorldSpaceCAmeraPos.xyz, but for the current view
                float3 worldSpaceViewerPos = UNITY_MATRIX_I_V._m03_m13_m23;

                // view forward
                float3 worldSpaceViewForward = -UNITY_MATRIX_I_V._m02_m12_m22;

                // pivot position
                float3 worldSpacePivotPos = unity_ObjectToWorld._m03_m13_m23;

                // offset between pivot and camera
                float3 worldSpacePivotToView = worldSpaceViewerPos - worldSpacePivotPos;

                // get the max object scale
                float3 scale = float3(
                    length(unity_ObjectToWorld._m00_m10_m20),
                    length(unity_ObjectToWorld._m01_m11_m21),
                    length(unity_ObjectToWorld._m02_m12_m22)
                );
                float maxScale = max(abs(scale.x), max(abs(scale.y), abs(scale.z)));

                // calculate a camera facing rotation matrix
                float3 up = UNITY_MATRIX_I_V._m01_m11_m21;
                float3 forward = isOrtho ? -worldSpaceViewForward : normalize(worldSpacePivotToView);
                float3 right = normalize(cross(forward, up));
                up = cross(right, forward);
                float3x3 quadOrientationMatrix = float3x3(right, up, forward);

                // use the max scale to figure out how big the quad needs to be to cover the entire sphere
                // we're using a hardcoded object space radius of 0.5 in the fragment shader
                float maxRadius = maxScale * 0.5;

                // find the radius of a cone that contains the sphere with the point at the camera and the base at the pivot of the sphere
                // this means the quad is always scaled to perfectly cover only the area the sphere is visible within
                float quadScale = maxScale;
                if (!isOrtho)
                {
                    // get the sine of the right triangle with the hyp of the sphere pivot distance and the opp of the sphere radius
                    float sinAngle = maxRadius / length(worldSpacePivotToView);
                    // convert to cosine
                    float cosAngle = sqrt(1.0 - sinAngle * sinAngle);
                    // convert to tangent
                    float tanAngle = sinAngle / cosAngle;

                    // basically this, but should be faster
                    //tanAngle = tan(asin(sinAngle));

                    // get the opp of the right triangle with the 90 degree at the sphere pivot * 2
                    quadScale = tanAngle * length(worldSpacePivotToView) * 2.0;
                }

                // flatten mesh, in case it's a cube or sloped quad mesh
                v.vertex.z = 0.0;

                // calculate world space position for the camera facing quad
                float3 worldPos = mul(v.vertex.xyz * quadScale, quadOrientationMatrix) + worldSpacePivotPos;

                // calculate world space view ray direction and origin for perspective or orthographic
                float3 worldSpaceRayOrigin = worldSpaceViewerPos;
                float3 worldSpaceRayDir = worldPos - worldSpaceRayOrigin;
                if (isOrtho)
                {
                    worldSpaceRayDir = worldSpaceViewForward * -dot(worldSpacePivotToView, worldSpaceViewForward);
                    worldSpaceRayOrigin = worldPos - worldSpaceRayDir;
                }

                // output object space ray direction and origin
                o.rayDir = mul(unity_WorldToObject, float4(worldSpaceRayDir, 0.0));
                o.rayOrigin = mul(unity_WorldToObject, float4(worldSpaceRayOrigin, 1.0));

                // offset towards the camera for use with conservative depth
            #if defined(USE_CONSERVATIVE_DEPTH)
                worldPos += worldSpaceRayDir / dot(normalize(worldSpacePivotToView), worldSpaceRayDir) * maxRadius;
            #endif

                o.pos = UnityWorldToClipPos(worldPos);

                return o;
            }

            // https://www.iquilezles.org/www/articles/spherefunctions/spherefunctions.htm
            float sphIntersect( float3 ro, float3 rd, float4 sph )
            {
                float3 oc = ro - sph.xyz;
                float b = dot( oc, rd );
                float c = dot( oc, oc ) - sph.w*sph.w;
                float h = b*b - c;
                if( h<0.0 ) return -1.0;
                h = sqrt( h );
                return -b - h;
            }

            #if defined(UNITY_PASS_FORWARDBASE) || defined(UNITY_PASS_FORWARDADD)
            // dummy struct to allow shadow macro to work
            struct shadowInput {
                SHADOW_COORDS(0)
            };

            half3 _LightColor0;

            half3 _Color;
            sampler2D _MainTex;
            samplerCUBE _CubeTex;

            // reuse the fragment shader for both forward base and forward add passes
            fixed4 frag_forward (v2f i
            #if defined(USE_CONSERVATIVE_DEPTH)
                , out float outDepth : SV_DepthLessEqual
            #else
            // the device probably can't use conservative depth
              , out float outDepth : SV_Depth
            #endif
                ) : SV_Target
            {
                // instancing
                // even though we're not using any instanced properties
                // we are using the unity_ObjectToWorld transform matrix 
                // and in instanced shaders, that needs the instance id
                UNITY_SETUP_INSTANCE_ID(i);

                // ray origin
                float3 rayOrigin = i.rayOrigin;

                // normalize ray vector
                float3 rayDir = normalize(i.rayDir);

                // ray sphere intersection
                float rayHit = sphIntersect(rayOrigin, rayDir, float4(0,0,0,0.5));

            #if !defined(_MSAABEHAVIOUR_ALPHATOCOVERAGE)
                // sphere intersection function returns -1 if there's no intersection
                clip(rayHit);
            #endif

                // cheap way to reduce mip map artifacts on edge
                // not 100% accurate, but close enough that it'd be hard to notice
                rayHit = rayHit < 0.0 ? dot(rayDir, -rayOrigin) : rayHit;

                // calculate object space position from ray, front hit ray length, and ray origin
                float3 surfacePos = rayDir * rayHit + rayOrigin;

                // object space surface normal
                float3 normal = normalize(surfacePos);

            #if defined(_MAPPING_CUBEMAP)
                // cubemap uvw
                float3 uvw = surfacePos;

                // swizzle & invert cubemap UVW so it matches equirectangular UVs
                uvw.xz = -uvw.zx;

                // sample cube map
                fixed4 col = texCUBE(_CubeTex, uvw);
            #else
                // -0.5 to 0.5 range
                float phi = atan2(normal.z, normal.x) / (UNITY_PI * 2.0);

                // 0.0 to 1.0 range
                float phi_frac = frac(phi);

                // negate the y because acos(-1.0) = PI, acos(1.0) = 0.0
                float theta = acos(-normal.y) / UNITY_PI;

                // construct the uvs, selecting the phi to use based on the derivatives
                float2 uv = float2(
                    fwidth(phi) < fwidth(phi_frac) - 0.001 ? phi : phi_frac,
                    theta
                    );

            #if MSAA_SUPER_SAMPLE
                // sample the equirectangular texture with an lod bias
                half4 col = tex2Dbias (_MainTex, float4(uv, 0, -1));
            #else
                // sample the equirectangular texture
                half4 col = tex2D (_MainTex, uv);
            #endif // MSAA_SUPER_SAMPLE
            #endif // _MAPPING_CUBEMAP

                // what shader is complete without a little color tinting?
                col.rgb *= _Color.rgb;

                // world space position and clip space position
                float3 worldPos = mul(unity_ObjectToWorld, float4(surfacePos, 1.0));
                float4 clipPos = UnityWorldToClipPos(float4(worldPos, 1.0));

                // stuff for directional shadow receiving
            #if defined (SHADOWS_SCREEN)
                // setup shadow struct for screen space shadows
                shadowInput shadowIN;
            #if defined(UNITY_NO_SCREENSPACE_SHADOWS)
                // mobile directional shadow
                shadowIN._ShadowCoord = mul(unity_WorldToShadow[0], float4(worldPos, 1.0));
            #else
                // screen space directional shadow
                shadowIN._ShadowCoord = ComputeScreenPos(clipPos);
            #endif // UNITY_NO_SCREENSPACE_SHADOWS
            #else
                // no shadow, or no directional shadow
                float shadowIN = 0;
            #endif // SHADOWS_SCREEN

                // basic lighting
                half3 worldNormal = UnityObjectToWorldNormal(normal);
                half3 worldLightDir = UnityWorldSpaceLightDir(worldPos);
                half ndotl = saturate(dot(worldNormal, worldLightDir));

                // get shadow, attenuation, and cookie
                UNITY_LIGHT_ATTENUATION(atten, shadowIN, worldPos);

                // per pixel lighting
                half3 lighting = _LightColor0 * ndotl * atten;

            #if defined(UNITY_SHOULD_SAMPLE_SH)
                // ambient lighting
                half3 ambient = ShadeSH9(float4(worldNormal, 1));
                lighting += ambient;

            #if defined(VERTEXLIGHT_ON)
                // "per vertex" non-important lights
                half3 vertexLighting = Shade4PointLights(
                unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
                unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
                unity_4LightAtten0, worldPos, worldNormal);

                lighting += vertexLighting;
            #endif // VERTEXLIGHT_ON
            #endif // UNITY_SHOULD_SAMPLE_SH

                // apply lighting
                col.rgb *= lighting;

                // output modified depth
                outDepth = clipPos.z / clipPos.w;
                
            #if !defined(UNITY_REVERSED_Z)
                // openGL platforms need the clip space to be rescaled
                outDepth = outDepth * 0.5 + 0.5;
            #endif

            #if defined(_MSAABEHAVIOUR_ALPHATOCOVERAGE)
                // ray to sphere origin distance
                float rayToPointDist = length(rayDir * dot(rayDir, -rayOrigin) + rayOrigin);

                // fwidth gets the sum of the ddx & ddy partial derivatives
                // float fDist = fwidth(rayToPointDist);

                // fwidth is a coarse approximation of this
                float fDist = length(float2(ddx(rayToPointDist), ddy(rayToPointDist)));

                // sharpen ray to point distance
                // centered on sphere radius, +/- half a pixel based on derivatives
                float alpha = (0.5 - rayToPointDist) / max(fDist, 0.0001) + 0.5;

                // clip based on sharpened alpha
                clip(alpha);

                // clamp alpha to a 0 to 1 range and apply to color
                col.a *= saturate(alpha);
            #endif

                // fog
                float fogCoord = clipPos.z;
            #if (SHADER_TARGET < 30) || defined(SHADER_API_MOBILE)
                // calculate fog falloff and creates a unityFogFactor variable to hold it
                UNITY_CALC_FOG_FACTOR(fogCoord);
                fogCoord = unityFogFactor;
            #endif
                UNITY_APPLY_FOG(fogCoord, col);

                return col;
            }
            #endif

            fixed4 frag_shadow (v2f i
            #if defined(USE_CONSERVATIVE_DEPTH)
                , out float outDepth : SV_DepthLessEqual
            #else
            // the device probably can't use conservative depth
              , out float outDepth : SV_Depth
            #endif
                ) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);

                // ray origin
                float3 rayOrigin = i.rayOrigin;

                // normalize ray vector
                float3 rayDir = normalize(i.rayDir);

                // ray sphere intersection
                float rayHit = sphIntersect(rayOrigin, rayDir, float4(0,0,0,0.5));

                // above function returns -1 if there's no intersection
                clip(rayHit);

                // calculate object space position from ray, front hit ray length, and ray origin
                float3 surfacePos = rayDir * rayHit + rayOrigin;

                // output modified depth
                float4 clipPos = UnityClipSpaceShadowCasterPos(surfacePos, surfacePos);
                clipPos = UnityApplyLinearShadowBias(clipPos);
                outDepth = clipPos.z / clipPos.w;

            #if !defined(UNITY_REVERSED_Z)
                // openGL platforms need the clip space to be rescaled
                outDepth = outDepth * 0.5 + 0.5;
            #endif

                return 0;
            }
        ENDCG

        Pass
        {
            Name "FORWARD"
            Tags { "LightMode" = "ForwardBase" }

            // remove this if you plan on using MSAA super sampling
            // for ease of demonstation purposes this is still enabled when super sampling is enabled
            // but really should only be enabled when using alpha to coverage
            AlphaToMask [_MSAABehaviour]

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag_forward

            // needed for conservative depth and sample modifier
            #pragma target 5.0

            #pragma multi_compile_fwdbase
            // skip support for any kind of baked lighting
            #pragma skip_variants LIGHTMAP_ON DYNAMICLIGHTMAP_ON DIRLIGHTMAP_COMBINED SHADOWS_SHADOWMASK
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #pragma shader_feature_local _ _MAPPING_CUBEMAP
            #pragma shader_feature_local _ _MSAABEHAVIOUR_ALPHATOCOVERAGE _MSAABEHAVIOUR_SUPERSAMPLE

            // this shouldn't be needed as this should be handled by the multi_compile_fwdbase
            // but I couldn't get it to use this variant without this line
            // might be because we're doing vertex lights in the fragment instead of vertex shader
            #pragma multi_compile _ VERTEXLIGHT_ON
            ENDCG
        }

        Pass
        {
            Name "FORWARD_ADD"
            Tags { "LightMode" = "ForwardAdd" }

            // remove this if you plan on using MSAA super sampling
            // for ease of demonstation purposes this is still enabled when super sampling is enabled
            // but really should only be enabled when using alpha to coverage
            AlphaToMask [_MSAABehaviour]

            Blend One One, Zero One
            ZWrite Off ZTest LEqual

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag_forward

            // needed for conservative depth and sample modifier
            #pragma target 5.0

            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #pragma shader_feature_local _ _MAPPING_CUBEMAP
            #pragma shader_feature_local _ _MSAABEHAVIOUR_ALPHATOCOVERAGE _MSAABEHAVIOUR_SUPERSAMPLE
            ENDCG
        }

        Pass
        {
            Name "SHADOWCASTER"
            Tags { "LightMode" = "ShadowCaster" }

            ZWrite On ZTest LEqual

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag_shadow

            // needed for conservative depth
            #pragma target 5.0

            #pragma multi_compile_shadowcaster
            #pragma multi_compile_instancing
            ENDCG
        }
    }
}