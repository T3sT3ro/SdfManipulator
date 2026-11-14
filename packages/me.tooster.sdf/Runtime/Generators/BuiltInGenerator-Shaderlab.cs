using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST;
using me.tooster.sdf.AST.Shaderlab.Syntax;
using me.tooster.sdf.AST.Shaderlab.Syntax.Commands;
using me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific;
using me.tooster.sdf.AST.Shaderlab.Syntax.SubShaderSpecific;
using me.tooster.sdf.AST.Syntax;
using me.tooster.sdf.AST.Syntax.CommonSyntax;
using UnityEngine;
using UnityEngine.Rendering;
using PropertySyntax = me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific.Property;
using Shader = me.tooster.sdf.AST.Shaderlab.Syntax.Shader;

namespace me.tooster.sdf.Generators {
    public partial class BuiltInGenerator {
        #region shaderlab

        Shader shader()
            => new()
            {
                name = scene.ShaderName,
                materialProperties = MaterialProperties(),
                shaderStatements = new ShaderStatement[]
                {
                    new Fallback { name = "Sdf/Fallback" },
                    // off, because Unity doesn't support UIToolkit yet and it's tedious to use UGUI
                    // new CustomEditor { editor = "me.tooster.sdf.Editor.Inspectors.SdfShaderEditor" },
                    SubShader(),
                },
            };

        /// <summary>
        ///     The material properties, which are the rendering settings of the scene. Controller values aren't material
        ///     properties: <see cref="SdfScene" /> sets them on its renderers with a <see cref="MaterialPropertyBlock" />, and
        ///     keeping them out of the shader means that changing a value doesn't change the shader (#114).
        /// </summary>
        static MaterialProperties MaterialProperties() => new() { properties = GlobalShaderProperties.ToList() };

        /// <summary>
        ///     The rendering settings. Properties that commands read (<c>Cull [_Cull]</c>) or that select keywords are
        ///     <c>Float</c>, as in Unity's own shaders, and integer uniforms are <c>Integer</c>. The legacy <c>Int</c> type
        ///     stores a float.
        /// </summary>
        static IEnumerable<PropertySyntax> GlobalShaderProperties {
            get {
                return new[]
                {
                    // [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull", Float) = 0
                    new PropertySyntax
                    {
                        id = "_Cull", displayName = "Cull", propertyType = new FloatKeyword(),
                        initializer = new Property.Number<IntLiteral> { numberLiteral = (int)CullMode.Off },
                        attributes = new[]
                        {
                            SyntaxUnityExtensions.headerAttribute("global raymarching properties"),
                            SyntaxUnityExtensions.spaceAttribute(),
                            SyntaxUnityExtensions.tooltipAttribute("turn OFF to raymarch inside object"),
                            SyntaxUnityExtensions.enumAttribute<CullMode>(),
                        },
                    },
                    // [Toggle][KeyEnum(Off, On)] _ZWrite ("ZWrite", Float) = 0
                    new PropertySyntax
                    {
                        id = "_ZWrite", displayName = "ZWrite", propertyType = new FloatKeyword(),
                        initializer = new Property.Number<IntLiteral> { numberLiteral = 1 },
                        attributes = new[]
                        {
                            SyntaxUnityExtensions.toggleAttribute(),
                            SyntaxUnityExtensions.keyEnumAttribute("Off", "On"),
                            SyntaxUnityExtensions.tooltipAttribute(
                                "Enable for correct blending with other geometry and backface rendering"
                            ),
                        },
                    },
                    // [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest ("ZTest", Float) = 4
                    new PropertySyntax
                    {
                        id = "_ZTest", displayName = "ZTest", propertyType = new FloatKeyword(),
                        initializer = new Property.Number<IntLiteral>
                            { numberLiteral = (int)CompareFunction.LessEqual },
                        attributes = SyntaxUnityExtensions.enumAttribute<CompareFunction>(),
                    },
                    // [KeywordEnum(Material, Albedo, Skybox, Normal, Steps, Depth)] _DrawMode ("Draw mode", Float) = 0
                    new PropertySyntax
                    {
                        id = "_DrawMode", displayName = "Draw mode", propertyType = new FloatKeyword(),
                        initializer = new Property.Number<IntLiteral> { numberLiteral = 0 },
                        attributes = new[]
                        {
                            SyntaxUnityExtensions.keywordEnumAttribute(
                                "Material",
                                "Albedo",
                                "Id",
                                "Skybox",
                                "Normal",
                                "Steps",
                                "Depth",
                                "Occlusion"
                            ),
                        },
                    },
                    // [Toggle(_SHOW_WORLD_GRID)] _ShowWorldGrid("Show World Grid overlay", Float) = 1
                    new PropertySyntax
                    {
                        id = "_SHOW_WORLD_GRID", displayName = "Show World Grid overlay", propertyType = new FloatKeyword(),
                        initializer = new Property.Number<IntLiteral> { numberLiteral = 1 },
                        attributes = new[]
                        {
                            SyntaxUnityExtensions.toggleAttribute("_SHOW_WORLD_GRID"),
                            SyntaxUnityExtensions.tooltipAttribute("Show world grid overlay"),
                        },
                    },
                    // _EpsilonRay ("epsilon step for ray to consider hit", Float) = 0.001
                    new PropertySyntax
                    {
                        id = "_EPSILON_RAY", displayName = "epsilon step for ray to consider hit", propertyType = new FloatKeyword(),
                        initializer = new Property.Number<FloatLiteral> { numberLiteral = 0.001f },
                    },
                    // _EPSILON_NORMAL ("epsilon for calculating normal", Float) = 0.001
                    new PropertySyntax
                    {
                        id = "_EPSILON_NORMAL", displayName = "epsilon for calculating normal", propertyType = new FloatKeyword(),
                        initializer = new Property.Number<FloatLiteral> { numberLiteral = 0.001f },
                    },
                    // _MAX_STEPS ("max raymarching steps", Integer) = 200
                    new PropertySyntax
                    {
                        id = "_MAX_STEPS", displayName = "max raymarching steps", propertyType = new IntegerKeyword(),
                        initializer = new Property.Number<IntLiteral> { numberLiteral = 200 },
                    },
                    // _MAX_DISTANCE ("marx raymarching distance", Float) = 200.0
                    new PropertySyntax
                    {
                        id = "_MAX_DISTANCE", displayName = "max raymarching distance", propertyType = new FloatKeyword(),
                        initializer = new Property.Number<FloatLiteral> { numberLiteral = 200.0f },
                    },
                    // _RAY_ORIGIN_BIAS ("ray origin bias", Float) = 0
                    new PropertySyntax
                    {
                        id = "_RAY_ORIGIN_BIAS", displayName = "ray origin bias", propertyType = new FloatKeyword(),
                        initializer = new Property.Number<FloatLiteral> { numberLiteral = 0.0f },
                    },
                    // _AO_STEPS ("Ambient Occlusion Steps", Integer) = 6;
                    new PropertySyntax
                    {
                        id = "_AO_STEPS", displayName = "Ambient Occlusion Steps", propertyType = new IntegerKeyword(),
                        initializer = new Property.Number<IntLiteral> { numberLiteral = 6 },
                    },
                    // _AO_MAX_DISTANCE ("Ambient Occlusion Max Distance") = 1;
                    new PropertySyntax
                    {
                        id = "_AO_MAX_DISTANCE", displayName = "Ambient Occlusion Max Distance", propertyType = new FloatKeyword(),
                        initializer = new Property.Number<FloatLiteral> { numberLiteral = 1.0f },
                    },
                    // _AO_FALLOFF ("Ambient Occlusion Falloff") = 2.5;
                    new PropertySyntax
                    {
                        id = "_AO_FALLOFF", displayName = "Ambient Occlusion Falloff", propertyType = new FloatKeyword(),
                        initializer = new Property.Number<FloatLiteral> { numberLiteral = 2.5f },
                    },
                };
            }
        }

        static TagsBlock Tags(params (string key, string value)[] tags)
            => new() { tags = tags.Select(t => new Tag { key = t.key, value = t.value }).ToList() };

        TagsBlock SubShaderTags => Tags(
            ("RenderType", transparent ? "Transparent" : "Opaque"),
            ("Queue", transparent ? "Transparent+1" : "Geometry+1"),
            ("IgnoreProjector", "True"),
            // batching moves vertices into world space, but rays and the shadow bias need the domain's own transform
            ("DisableBatching", "True")
        );

        /// <summary>
        ///     The include block that every pass shares: library includes, uniforms and the scene functions. Each pass then
        ///     only selects its entry points and keywords.
        /// </summary>
        SubShader SubShader()
            => new()
            {
                statements = new List<SubShaderOrPassStatement>
                {
                    SubShaderTags,
                    new HlslInclude { hlsl = new InjectedLanguage<shaderlab, hlsl>(CommonHlslInclude()) },
                    ForwardBasePass(),
                    ForwardAddPass(),
                    ShadowCasterPass(),
                    SceneSelectionPass(),
                    PickingPass(),
                    ObjectPickingPass(),
                },
            };

        static CalculatedArgument MaterialProperty(string id) => new() { id = id };

        /// <summary>The main directional light, ambient light, reflections and the debug draw modes.</summary>
        Pass ForwardBasePass()
            => new()
            {
                statements = new List<SubShaderOrPassStatement>
                    {
                        new Pass.Name { name = "FORWARD" },
                        Tags(("LightMode", "ForwardBase")),
                        new ZTest { operation = MaterialProperty("_ZTest") },
                        new Cull { state = MaterialProperty("_Cull") },
                        new ZWrite { state = MaterialProperty("_ZWrite") },
                        transparent
                            ? new Blend
                            {
                                blendArguments = new Blend.SrcDstArguments
                                {
                                    srcFactorArg = new SrcAlphaKeyword(),
                                    dstFactorArg = new OneMinusSrcAlphaKeyword(),
                                },
                            }
                            : null,
                        PassProgram(
                            new[]
                            {
                                // SDF surfaces have no lightmap coordinates, and the light input that a raymarched hit
                                // builds can't hold lightmap and shadow mask coordinates
                                "multi_compile_fwdbase nolightmap nodynlightmap nodirlightmap",
                                "skip_variants SHADOWS_SHADOWMASK",
                                "multi_compile_fog",
                            }.Concat(pragmas)
                        ),
                    }.FilterNotNull()
                    .ToList(),
            };

        /// <summary>One pass for each additional pixel light, added to the forward base pass.</summary>
        Pass ForwardAddPass()
            => new()
            {
                statements = new List<SubShaderOrPassStatement>
                {
                    new Pass.Name { name = "FORWARD_DELTA" },
                    Tags(("LightMode", "ForwardAdd")),
                    new ZTest { operation = MaterialProperty("_ZTest") },
                    new Cull { state = MaterialProperty("_Cull") },
                    new ZWrite { state = new OffKeyword() },
                    new Blend
                    {
                        blendArguments = new Blend.SrcDstArguments { srcFactorArg = new OneKeyword(), dstFactorArg = new OneKeyword() },
                    },
                    PassProgram(
                        "multi_compile_fwdadd_fullshadows",
                        "skip_variants SHADOWS_SHADOWMASK",
                        "multi_compile_fog",
                        "vertex forwardAddVertex",
                        "fragment forwardAddFragment"
                    ),
                },
            };

        /// <summary>
        ///     Writes the raymarched depth into shadow maps and into the camera depth texture, from which directional
        ///     lights build screen-space shadows.
        /// </summary>
        Pass ShadowCasterPass()
            => new()
            {
                statements = new List<SubShaderOrPassStatement>
                {
                    new Pass.Name { name = "ShadowCaster" },
                    Tags(("LightMode", "ShadowCaster")),
                    // the default depth test, LEqual
                    new Cull { state = MaterialProperty("_Cull") },
                    new ZWrite { state = new OnKeyword() },
                    PassProgram("multi_compile_shadowcaster", "vertex shadowCasterVertex", "fragment shadowCasterFragment"),
                },
            };

        /// <summary>Draws the outline of a selected scene in the scene view along the SDF surfaces.</summary>
        Pass SceneSelectionPass()
            => new()
            {
                statements = new List<SubShaderOrPassStatement>
                {
                    new Pass.Name { name = "SceneSelectionPass" },
                    Tags(("LightMode", "SceneSelectionPass")),
                    // rays start on the near plane, so any face of the domain mesh covers the pixels
                    new Cull { state = new OffKeyword() },
                    PassProgram("vertex sceneSelectionVertex", "fragment sceneSelectionFragment"),
                },
            };

        /// <summary>Lets the scene view pick the renderer only where its rays hit the SDF.</summary>
        Pass PickingPass()
            => new()
            {
                statements = new List<SubShaderOrPassStatement>
                {
                    new Pass.Name { name = "ScenePickingPass" },
                    Tags(("LightMode", "Picking")),
                    new Cull { state = new OffKeyword() },
                    PassProgram("vertex pickingVertex", "fragment pickingFragment"),
                },
            };

        /// <summary>
        ///     Writes a picking index for each object, with which the scene view selects the object under the cursor
        ///     instead of the whole scene. Unity doesn't draw this pass by itself: the editor draws it while it picks.
        /// </summary>
        Pass ObjectPickingPass()
            => new()
            {
                statements = new List<SubShaderOrPassStatement>
                {
                    new Pass.Name { name = "SdfObjectPicking" },
                    Tags(("LightMode", "SdfObjectPicking")),
                    new Cull { state = new OffKeyword() },
                    PassProgram("vertex objectPickingVertex", "fragment objectPickingFragment"),
                },
            };

        HlslProgram PassProgram(params string[] passPragmas) => PassProgram(passPragmas.AsEnumerable());

        HlslProgram PassProgram(IEnumerable<string> passPragmas)
            => new() { hlsl = new InjectedLanguage<shaderlab, hlsl>(PassHlslTree(passPragmas)) };

        #endregion
    }
}
