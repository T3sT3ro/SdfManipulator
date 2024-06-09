using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST;
using me.tooster.sdf.AST.Shaderlab.Syntax;
using me.tooster.sdf.AST.Shaderlab.Syntax.Commands;
using me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific;
using me.tooster.sdf.AST.Shaderlab.Syntax.SubShaderSpecific;
using me.tooster.sdf.AST.Syntax;
using me.tooster.sdf.AST.Syntax.CommonSyntax;
using me.tooster.sdf.AST.Syntax.CommonTrivia;
using me.tooster.sdf.Editor.Controllers.SDF;
using me.tooster.sdf.Editor.Util;
using Unity.Properties;
using UnityEngine;
using PropertySyntax = me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific.Property;
using Shader = me.tooster.sdf.AST.Shaderlab.Syntax.Shader;

namespace me.tooster.sdf.Editor.Controllers.Generators {
    public partial class BuiltInGenerator {
        #region shaderlab

        Shader shader()
            => new()
            {
                name = scene.name + " (generated)",
                materialProperties = MaterialProperties(),
                shaderStatements = new ShaderStatement[]
                {
                    new Fallback { name = "Sdf/Fallback" },
                    // off, because Unity doesn't support UIToolkit yet and it's tedious to use UGUI
                    // new CustomEditor { editor = "me.tooster.sdf.Editor.Controllers.Editors.SdfShaderEditor" },
                    SubShader(),
                },
            };

        /// <summary>
        ///  generates a material properties block from collected properties
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">property type is not supported</exception>
        MaterialProperties MaterialProperties() {
            return new MaterialProperties
            {
                properties = GlobalShaderProperties
                    .Concat(
                        scene.sceneData.Properties
                            .Where(p => PropertyContainer.GetProperty(p.controller, p.path).IsPropertyShaderlabCompatible())
                            .GroupBy(pd => pd.controller)
                            .SelectMany(
                                group => group.Select(generatePropertySyntax)
                                    .Select(
                                        (ps, i) => i > 0
                                            ? ps
                                            : ps with
                                            {
                                                attributes = SyntaxUnityExtensions.headerAttribute(
                                                    scene.GetControllerSceneAncestors(group.Key).Reverse().Select(c => c.name)
                                                        .JoinToString("/")
                                                ).WithLeadingTrivia(new NewLine<shaderlab>(), new NewLine<shaderlab>()),
                                            }
                                    )
                            )
                    )
                    .ToList(),
            };
        }

        static IEnumerable<PropertySyntax> GlobalShaderProperties {
            get {
                return new[]
                {
                    // [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull", Int) = 0
                    new PropertySyntax
                    {
                        id = "_Cull", displayName = "Cull", propertyType = new IntKeyword(),
                        initializer = new PropertySyntax.Number<IntLiteral> { numberLiteral = (int)UnityEngine.Rendering.CullMode.Back },
                        attributes = new[]
                        {
                            SyntaxUnityExtensions.headerAttribute("global raymarching properties"),
                            SyntaxUnityExtensions.spaceAttribute(),
                            SyntaxUnityExtensions.enumAttribute<UnityEngine.Rendering.CullMode>(),
                        },
                    },
                    // [Toggle][KeyEnum(Off, On)] _ZWrite ("ZWrite", Float) = 0
                    new PropertySyntax
                    {
                        id = "_ZWrite", displayName = "ZWrite", propertyType = new FloatKeyword(),
                        initializer = new PropertySyntax.Number<IntLiteral> { numberLiteral = 1 },
                        attributes = new[]
                        {
                            SyntaxUnityExtensions.toggleAttribute(),
                            SyntaxUnityExtensions.keyEnumAttribute("Off", "On"),
                            SyntaxUnityExtensions.tooltipAttribute(
                                "Enable for correct blending with other geometry and backface rendering"
                            ),
                        },
                    },
                    // [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Int) = 1
                    new PropertySyntax
                    {
                        id = "_ZTest", displayName = "ZTest", propertyType = new IntKeyword(),
                        initializer = new PropertySyntax.Number<IntLiteral>
                            { numberLiteral = (int)UnityEngine.Rendering.CompareFunction.LessEqual },
                        attributes = SyntaxUnityExtensions.enumAttribute<UnityEngine.Rendering.CompareFunction>(),
                    },
                    // [KeywordEnum(Material, Albedo, Skybox, Normal, Steps, Depth)] _DrawMode("Draw mode", Int) = 0
                    new PropertySyntax()
                    {
                        id = "_DrawMode", displayName = "Draw mode", propertyType = new IntKeyword(),
                        initializer = new PropertySyntax.Number<IntLiteral> { numberLiteral = 0 },
                        attributes = new[]
                        {
                            SyntaxUnityExtensions.keywordEnumAttribute(
                                "Material",
                                "Albedo",
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
                        initializer = new PropertySyntax.Number<IntLiteral> { numberLiteral = 1 },
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
                        initializer = new PropertySyntax.Number<FloatLiteral> { numberLiteral = 0.001f },
                    },
                    // _EPSILON_NORMAL ("epsilon for calculating normal", Float) = 0.001
                    new PropertySyntax
                    {
                        id = "_EPSILON_NORMAL", displayName = "epsilon for calculating normal", propertyType = new FloatKeyword(),
                        initializer = new PropertySyntax.Number<FloatLiteral> { numberLiteral = 0.001f },
                    },
                    // _MAX_STEPS ("max raymarching steps", Int) = 200
                    new PropertySyntax
                    {
                        id = "_MAX_STEPS", displayName = "max raymarching steps", propertyType = new IntKeyword(),
                        initializer = new PropertySyntax.Number<IntLiteral> { numberLiteral = 200 },
                    },
                    // _MAX_DISTANCE ("marx raymarching distance", Float) = 200.0
                    new PropertySyntax
                    {
                        id = "_MAX_DISTANCE", displayName = "max raymarching distance", propertyType = new FloatKeyword(),
                        initializer = new PropertySyntax.Number<FloatLiteral> { numberLiteral = 200.0f },
                    },
                    // _RAY_ORIGIN_BIAS ("ray origin bias", Float) = 0
                    new PropertySyntax
                    {
                        id = "_RAY_ORIGIN_BIAS", displayName = "ray origin bias", propertyType = new FloatKeyword(),
                        initializer = new PropertySyntax.Number<FloatLiteral> { numberLiteral = 0.0f },
                    },
                    // _AO_STEPS ("Ambient Occlusion Steps", Integer) = 6;
                    new PropertySyntax
                    {
                        id = "_AO_STEPS", displayName = "Ambient Occlusion Steps", propertyType = new IntKeyword(),
                        initializer = new PropertySyntax.Number<IntLiteral> { numberLiteral = 6 },
                    },
                    // _AO_MAX_DISTANCE ("Ambient Occlusion Max Distance") = 1;
                    new PropertySyntax
                    {
                        id = "_AO_MAX_DISTANCE", displayName = "Ambient Occlusion Max Distance", propertyType = new FloatKeyword(),
                        initializer = new PropertySyntax.Number<FloatLiteral> { numberLiteral = 1.0f },
                    },
                    // _AO_FALLOFF ("Ambient Occlusion Falloff") = 2.5;
                    new PropertySyntax
                    {
                        id = "_AO_FALLOFF", displayName = "Ambient Occlusion Falloff", propertyType = new FloatKeyword(),
                        initializer = new PropertySyntax.Number<FloatLiteral> { numberLiteral = 2.5f },
                    },
                };
            }
        }

        PropertySyntax generatePropertySyntax(SdfScene.PropertyData pd) {
            var t = pd.property.DeclaredValueType();

            var shaderlabType = t.shaderlabTypeKeyword();
            PropertySyntax.Initializer shaderlabInitializer = t switch
            {
                not null when t == typeof(int) => new PropertySyntax.Number<IntLiteral>
                    { numberLiteral = PropertyContainer.GetValue<Controller, int>(pd.controller, pd.path) },
                not null when t == typeof(float) => new PropertySyntax.Number<FloatLiteral>
                    { numberLiteral = PropertyContainer.GetValue<Controller, float>(pd.controller, pd.path) },
                not null when t == typeof(Vector2) => new PropertySyntax.Vector
                    { arguments = PropertyContainer.GetValue<Controller, Vector2>(pd.controller, pd.path).VectorArgumentList() },
                not null when t == typeof(Vector3) => new PropertySyntax.Vector
                    { arguments = PropertyContainer.GetValue<Controller, Vector3>(pd.controller, pd.path).VectorArgumentList() },
                not null when t == typeof(Vector4) => new PropertySyntax.Vector
                    { arguments = PropertyContainer.GetValue<Controller, Vector4>(pd.controller, pd.path).VectorArgumentList() },
                not null when t == typeof(Vector2Int) => new PropertySyntax.Vector
                    { arguments = PropertyContainer.GetValue<Controller, Vector2Int>(pd.controller, pd.path).VectorArgumentList() },
                not null when t == typeof(Vector3Int) => new PropertySyntax.Vector
                    { arguments = PropertyContainer.GetValue<Controller, Vector3Int>(pd.controller, pd.path).VectorArgumentList() },
                _ => throw new ShaderGenerationException($"Can't generate initializer for shaderlab property {pd}"),
            };


            return new PropertySyntax
                {
                    propertyType = shaderlabType,
                    initializer = shaderlabInitializer,
                } with
                {
                    id = pd.identifier,
                    displayName = PropertyContainer.GetProperty(pd.controller, pd.path)
                        .GetAttribute<ShaderPropertyAttribute>().Description,
                };
        }

        TagsBlock TagsBlock => new()
        {
            tags = new[]
            {
                new Tag { key = "RenderType", value = transparent ? "Transparency" : "Opaque" },
                new Tag { key = "Queue", value = transparent ? "Transparent+1" : "Geometry+1" },
                new Tag { key = "IgnoreProjector", value = "True" },
                new Tag { key = "LightMode", value = "ForwardBase" },
            },
        };

        IEnumerable<SubShaderOrPassStatement> Commands => new List<Command>
        { // TODO: use properties on raymarcher node to set these possibly? 
            new ZTest { operation = new CalculatedArgument { id = "_ZTest" } },
            new Cull { state = new CalculatedArgument { id = "_Cull" } },
            new ZWrite { state = new CalculatedArgument { id = "_ZWrite" } },
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
        }.FilterNotNull();

        SubShader SubShader()
            =>
                // scene.sdfSceneRoot!.Apply(SdfData.pData, this); // ignore retun, only handle requirements
                new()
                {
                    statements = new SyntaxList<shaderlab, SubShaderOrPassStatement>(TagsBlock)
                        .Splice(1, 0, Commands)
                        .Append<SubShaderOrPassStatement>(
                            new HlslInclude
                            {
                                hlsl = new InjectedLanguage<shaderlab, hlsl>(CommonHlslInclude()),
                            }
                        )
                        .Append(Pass())
                        .ToList(),
                };

        Pass Pass()
            => new()
            {
                statements = new PassStatement[]
                {
                    new HlslProgram { hlsl = new InjectedLanguage<shaderlab, hlsl>(HlslTree()) },
                },
            };

        #endregion
    }
}
