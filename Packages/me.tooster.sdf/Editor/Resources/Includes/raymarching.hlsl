#pragma once

/// calculate rays in the vertex shader
#define V2F_RAYS

#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "UnityPBSLighting.cginc"
#include "AutoLight.cginc"

#include "Packages/me.tooster.sdf/Editor/Resources/Includes/types.hlsl"
#include "Packages/me.tooster.sdf/Editor/Resources/Includes/types.hlsl"
#include "Packages/me.tooster.sdf/Editor/Resources/Includes/camera.hlsl"
#include "Packages/me.tooster.sdf/Editor/Resources/Includes/debug.hlsl"
#include "Packages/me.tooster.sdf/Editor/Resources/Includes/noise.hlsl"
#include "Packages/me.tooster.sdf/Editor/Resources/Includes/colors.hlsl"

/// data passed from vertex to fragment shader
struct v2f {
    /// clip space vertex pos. Space in vertex shader: (-w, -w, 0)->(w,w,w). In fragment (after hardware perspective divide) is (-1, -1, 0*) to (1,1,1*). Z value depends on platform.
    float4 vertex : SV_POSITION;
    float4 screenPos: TEXCOORD1; ///< clip-pos vertex position. This is not perspective divided, so that `w` is not lost beetween stages
    float3 hitpos : TEXCOORD2; ///< hit position in model space TODO: remove

    #ifdef V2F_RAYS
    /** \brief unnormalized ray direction in world space. \code noperspective \endcode is needed to prevent warping of direction vectors
     * \remark <a href="https://www.geogebra.org/calculator/fppufpcf">needed for proper interpolation</a>
     * \remark <a href="https://mathweb.ucsd.edu/~sbuss/MathCG2/OpenGLsoft/NoPerspective/docNoPerspective.html">how perpective divition works: opengl</a>
     * \remark <a href="https://docs.vulkan.org/spec/latest/chapters/primsrast.html#primsrast-polygons-basic:~:text=c%2C%20respectively.-,Perspective%20interpolation,-for%20a%20triangle">how perpective divition works: vulkan</a>
     */
    noperspective float3 rdWsUnnormalized : TEXCOORD3;
    /// ray origin in world space. \code noperspective \endcode is needed to prevent warping and bad artifacts inside domain
    noperspective float3 roWs : TEXCOORD4; ///< ray origin in world space
    #endif
};

#pragma region Forward declarations

/// A scene SignedDistanceField function, defined in the included shader
SdfResult sdfScene(in float3 p);

SdfResult raymarch(inout Ray3D ray, in int max_steps, in float epsilon_ray);
float3    calculateSdfNormal(in float3 p, in float epsilon_normal);
float     depthToMaxRayDepth(in Texture2D depthTexture, in float2 screenUV, in float3 rd, in float4x4 inv);
float     normalSdfOcclusion(in float3 p, in float3 normal, in int i, in float step_size);
float     classicAmbientOcclusion(float3 p, float3 n, float maxDist, float falloff, in int aoSteps);

#pragma endregion

#ifndef EXPLICIT_RAYMARCHING_PARAMETERS
float _EPSILON_RAY = 0.00001;
float _EPSILON_NORMAL = 0.0001;
float _MAX_STEPS = 500;
float _MAX_DISTANCE = 10000;
float _RAY_ORIGIN_BIAS = 0;
UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
#endif

#ifndef EXPLICIT_SHADER_FEATURES
#pragma shader_feature_local \
    _DRAWMODE_MATERIAL \
    _DRAWMODE_ALBEDO \
    _DRAWMODE_ID \
    _DRAWMODE_SKYBOX \
    _DRAWMODE_NORMAL \
    _DRAWMODE_STEPS \
    _DRAWMODE_DEPTH \
    _DRAWMODE_OCCLUSION
#pragma shader_feature_local __ _SHOW_WORLD_GRID
#endif

# pragma region raymarching functions

/**
 * \brief reads the _CameraDepthTexture (rendered BEFORE this shader!) and returns the distance along rd to reach for that depth
 * \param depthTexture camera depth texture to read from the and determine maximum ray depth
 * \param screenUV a (0,0)->(1,1) screen-space coordinate of the pixel to read depth from
 * \param rd a ray direction from the camera
 * \param inv an inverse projection+world+model* matrix. *depends on the local frame 
 * \return returns a distance along the ray rd to reach for the depth at screenUV
 * \remark TODO: simplify to avoid inverse projection and both rd and uv
 */
float depthToMaxRayDepth(in sampler2D depthTexture, in float2 screenUV, in float3 rd, in float4x4 inv) {
    // read camera depth texture to correctly blend with scene geometry
    // beware, that _CameraDepthTexture IS NOT the depth buffer!
    // it is populated in the prepass and doesn't change in subsequent passes
    // https://forum.unity.com/threads/does-depth-buffer-update-between-passes.620575/
    float camDepth = CorrectDepth(tex2D(depthTexture, screenUV).rg);

    float4 forward = mul(inv, float4(0, 0, 1, 1)); // central ray end on far plane
    forward /= forward.w;
    forward = normalize(forward); // forward in object space
    return camDepth / dot(forward, rd);
}

// https://iquilezles.org/www/articles/normalsSDF/normalsSDF.htm
float3 sampleNormal(in float3 p, in float epsilon_normal) {
    // using tetrahedron technique
    // EPSILON -- can be adjusted using pixel footprint
    const float2 k = float2(1, -1);
    return normalize(
        k.xyy * sdfScene(p + k.xyy * epsilon_normal).distance +
        k.yyx * sdfScene(p + k.yyx * epsilon_normal).distance +
        k.yxy * sdfScene(p + k.yxy * epsilon_normal).distance +
        k.xxx * sdfScene(p + k.xxx * epsilon_normal).distance
    );
}

// returns sdf and ray point
SdfResult raymarch(inout Ray3D ray, in int max_steps, in float epsilon_ray) {
    SdfResult sdf = (SdfResult)0;
    sdf.distance = ray.maxDistance;
    sdf.id = int4(NO_ID);

    sdf.material = (Material)0;
    sdf.normal = 0;
    // ray can start inside SDF, this implementation makes it perform one step onto the surface
    // is it good or bad, well, depends on the use case 
    for (ray.steps = 0; ray.steps < max_steps; ray.steps++) {
        if (ray.marchedDistance >= ray.maxDistance) // there was a check before for ray.max distance OR _MAX_DISTANCE... but it can just be done in the outer scope
            return sdf;

        sdf.p = ray.ro + ray.marchedDistance * ray.rd;

        SdfResult sdfOnRay = sdfScene(sdf.p);
        if (sdfOnRay.distance < epsilon_ray) {
            sdf.distance = ray.marchedDistance;
            sdf.id = sdfOnRay.id;
            return sdf;
        }

        ray.marchedDistance += sdfOnRay.distance;
    }
    return sdf;
}

/**
 * \brief calculates normal as a gradient of the sdf using tetrahedron technique
 * \remarks see <a href="https://iquilezles.org/www/articles/normalsSDF/normalsSDF.htm">calculating normals by iq</a>
 */
float3 calculateSdfNormal(in float3 p, in float epsilon_normal) {
    // using tetrahedron technique
    // EPSILON -- can be adjusted using pixel footprint
    const float2 k = float2(1, -1);
    return normalize(
        k.xyy * sdfScene(p + k.xyy * epsilon_normal).distance +
        k.yyx * sdfScene(p + k.yyx * epsilon_normal).distance +
        k.yxy * sdfScene(p + k.yxy * epsilon_normal).distance +
        k.xxx * sdfScene(p + k.xxx * epsilon_normal).distance
    );
}

#pragma endregion

# pragma region shading

/**
 * \brief Calculates simple occlusion
 * \param p point to calculate sdf occlusion for
 * \param normal normal at point p
 * \param i number of occlusion steps
 * \return occlusion in a range [0 (none) ... 1 (full)]
 * \remarks <a href="https://typhomnt.github.io/teaching/ray_tracing/raymarching_intro/">occlusion by typhomnt</a>
 */
float normalSdfOcclusion(in float3 p, in float3 normal, in int i, in float step_size) {
    float occlusion = 1.0;
    while (i > 0) { // steps used as a counter
        occlusion -= pow(i * i - sdfScene(p + normal * i * step_size).distance, 2) / i;
        i--;
    }
    return occlusion;
}

/**
 * Calculates ambient occlusion
 * @param p point to calculate sdf occlusion for
 * @param n normal at point p
 * @param maxDist max distance for occlusion
 * @param falloff how much to reduce the occlusion
 * @param aoSteps how many steps to apply, 6 is a godo tradeoff between quality and performance
 * @return an occlusion in a range [0 (none) ... 1 (full)]
 * @remarks <a href="https://www.shadertoy.com/view/4sdGWN">occlusion by XT95</a>
 */
float classicAmbientOcclusion(float3 p, float3 n, float maxDist, float falloff, in int aoSteps) {
    float ao = 0.0;
    for (int i = 0; i < aoSteps; i++) {
        float  l = noise::hash(float(i)) * maxDist;
        float3 rd = n * l;

        ao += (l - max(sdfScene(p + rd).distance, 0.0)) / maxDist * falloff;
    }

    return clamp(1.0 - ao / float(aoSteps), 0.0, 1.0);
}

/**
 * 
 * @param color normal shaded color of the fragment, usually the result of regular shading function like Phong, Lambert or BRDF
 * @param mat material data of fragment
 * @param sdf sdf data of fragment
 * @param ray ray info for fragment
 * @return modified color of a fragment
 * @todo check if including this and shader features in global include file increases shader compilation time
 */
fixed4 debugOverlay(in fixed4 color, in Material mat, in SdfResult sdf, in Ray3D ray) {
    #ifdef _DRAWMODE_MATERIAL
    // dampen the color by occlusion
    color = color;
    #elif _DRAWMODE_ALBEDO
    color = mat.albedo;
    #elif _DRAWMODE_ID
    color = noise::randomColor(sdf.id.w); // TODO: account for xyz
    #elif _DRAWMODE_SKYBOX
    // sample the default reflection cubemap, using the reflection vector
    half4 skyData = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, sdf.normal);
    // decode cubemap data into actual color
    half3 skyColor = DecodeHDR(skyData, unity_SpecCube0_HDR);
    color = fixed4(skyColor, 1.0);
    #elif _DRAWMODE_NORMAL
    color = sdf::debug::visualizeNormal(sdf.normal);
    #elif _DRAWMODE_STEPS
    color = sdf::debug::visualizeOverhead(ray.steps/_MAX_STEPS);
    #elif _DRAWMODE_DEPTH
    float eyeDepth = -UnityWorldToViewPos(sdf.p).z;
    color = fixed4((float3)eyeDepth/5., 1);
    #elif _DRAWMODE_OCCLUSION
    color = mat.occlusion;
    #endif

    #ifdef _SHOW_WORLD_GRID
    fixed4 gridColor = sdf::debug::grid(sdf.p, 0.25, .04);
    color.rgb = lerp(color, gridColor.rgb, gridColor.a); // mix in the grid
    #endif

    return color;
}

#pragma endregion
