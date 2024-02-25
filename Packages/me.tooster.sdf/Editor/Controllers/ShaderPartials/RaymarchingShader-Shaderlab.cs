using System;
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST;
using me.tooster.sdf.AST.Shaderlab;
using me.tooster.sdf.AST.Shaderlab.Syntax;
using me.tooster.sdf.AST.Shaderlab.Syntax.Commands;
using me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific;
using me.tooster.sdf.AST.Shaderlab.Syntax.SubShaderSpecific;
using me.tooster.sdf.AST.Syntax;
using me.tooster.sdf.AST.Syntax.CommonSyntax;
using me.tooster.sdf.Editor.API;
using me.tooster.sdf.Editor.Controllers.SDF;
using me.tooster.sdf.Editor.Util;
using UnityEngine;
using Attribute = me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific.Attribute;
using Property = me.tooster.sdf.Editor.API.Property;
using PropertySyntax = me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific.Property;
using Shader = me.tooster.sdf.AST.Shaderlab.Syntax.Shader;

namespace me.tooster.sdf.Editor.Controllers.ShaderPartials {
    public partial class RaymarchingShader {
        [SerializeField] private bool transparent = false;

        public string MainShader(SdfScene scene) {
            var unformatedSource = shader(scene);
            var formattedSource = ShaderlabFormatter.Format(unformatedSource);
            return formattedSource?.ToString() ?? "// empty shader";
        }

        #region shaderlab

        private Shader shader(SdfScene scene) => new()
        {
            name = scene.name + " (generated)",
            materialProperties = MaterialProperties(scene),
            shaderStatements = new ShaderStatement[]
            {
                new Fallback { name = "Sdf/Fallback" },
                new CustomEditor { editor = "me.tooster.sdf.Editor.Controllers.Editors.SdfSceneShaderEditor" },
                SubShader(scene),
            },
        };

        /// <summary>
        ///  generates a material properties block from collected properties
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">property type is not supported</exception>
        private MaterialProperties MaterialProperties(SdfScene scene) {
            return new MaterialProperties
            {
                properties = GlobalShaderProperties
                    .Concat(scene.Properties
                        .SelectMany(properties => properties
                            .Where(p => p.IsPropertyShaderlabCompatible())
                            .Select(p => generatePropertySyntax(scene, p) with
                            {
                                attributes = SyntaxExtensions.headerAttribute(
                                    scene.GetControllerPath(properties.Key).Select(c => c.name).JoinToString("/")),
                            })))
                    .ToList(),
            };
        }

        private static IEnumerable<PropertySyntax> GlobalShaderProperties {
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
                            SyntaxExtensions.headerAttribute("global raymarching properties"),
                            SyntaxExtensions.spaceAttribute(),
                            SyntaxExtensions.enumAttribute<UnityEngine.Rendering.CullMode>(),
                        },
                    },
                    // [Toggle][KeyEnum(Off, On)] _ZWrite ("ZWrite", Float) = 0
                    new PropertySyntax
                    {
                        id = "_ZWrite", displayName = "ZWrite", propertyType = new FloatKeyword(),
                        initializer = new PropertySyntax.Number<IntLiteral> { numberLiteral = 1 },
                        attributes = new[]
                        {
                            SyntaxExtensions.tooltipAttribute("Enable for correct blending with other geometry and backface rendering"),
                            SyntaxExtensions.toggleAttribute(),
                            SyntaxExtensions.keyEnumAttribute("Off", "On"),
                        },
                    },
                    // [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Int) = 1
                    new PropertySyntax
                    {
                        id = "_ZTest", displayName = "ZTest", propertyType = new IntKeyword(),
                        initializer = new PropertySyntax.Number<IntLiteral>
                            { numberLiteral = (int)UnityEngine.Rendering.CompareFunction.LessEqual },
                        attributes = SyntaxExtensions.enumAttribute<UnityEngine.Rendering.CompareFunction>(),
                    },
                };
            }
        }

        private PropertySyntax generatePropertySyntax(SdfScene scene, Property property) => property switch
            {
                Property<int> p => new PropertySyntax
                {
                    propertyType = new PropertySyntax.PredefinedType { type = new IntKeyword() },
                    initializer = new PropertySyntax.Number<IntLiteral> { numberLiteral = p.DefaultValue },
                },
                Property<float> p => new PropertySyntax
                {
                    propertyType = new PropertySyntax.PredefinedType { type = new FloatKeyword() },
                    initializer = new PropertySyntax.Number<FloatLiteral> { numberLiteral = p.DefaultValue },
                },
                Property<Vector4> p => new PropertySyntax
                {
                    propertyType = new PropertySyntax.PredefinedType { type = new VectorKeyword() },
                    initializer = new PropertySyntax.Vector { arguments = p.DefaultValue.VectorArgumentList() },
                },
                Property<Vector3> p => new PropertySyntax
                {
                    propertyType = new PropertySyntax.PredefinedType { type = new VectorKeyword() },
                    initializer = new PropertySyntax.Vector { arguments = p.DefaultValue.VectorArgumentList() },
                },
                Property<Vector2> p => new PropertySyntax
                {
                    propertyType = new PropertySyntax.PredefinedType { type = new VectorKeyword() },
                    initializer = new PropertySyntax.Vector { arguments = p.DefaultValue.VectorArgumentList() },
                },
                Property<Vector3Int> p => new PropertySyntax
                {
                    propertyType = new PropertySyntax.PredefinedType { type = new VectorKeyword() },
                    initializer = new PropertySyntax.Vector { arguments = p.DefaultValue.VectorArgumentList() },
                },
                Property<Vector2Int> p => new PropertySyntax
                {
                    propertyType = new PropertySyntax.PredefinedType { type = new VectorKeyword() },
                    initializer = new PropertySyntax.Vector { arguments = p.DefaultValue.VectorArgumentList() },
                },
                _ => throw new ArgumentOutOfRangeException(nameof(property),
                    $"Material property block can't expose property of type {property.GetType()}"),
            } with
            {
                id = scene.GetPropertyIdentifier(property),
                displayName = property.DisplayName,
            };

        private TagsBlock TagsBlock => new()
        {
            tags = new[]
            {
                new Tag { key = "RenderType", value = transparent ? "Transparency" : "Opaque" },
                new Tag { key = "Queue", value = transparent ? "Transparent+1" : "Geometry+1" },
                new Tag { key = "IgnoreProjector", value = "True" },
                new Tag { key = "LightMode", value = "ForwardBase" },
            },
        };

        private IEnumerable<SubShaderOrPassStatement> Commands => new List<Command>
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

        private SubShader SubShader(SdfScene scene) => new()
        {
            statements = new SyntaxList<shaderlab, SubShaderOrPassStatement>(TagsBlock)
                .Splice(1, 0, Commands)
                .Append<SubShaderOrPassStatement>(Pass(scene))
                .ToList(),
        };

        private Pass Pass(SdfScene scene) => new()
        {
            statements = new PassStatement[]
            {
                new HlslProgram { hlsl = new InjectedLanguage<shaderlab, hlsl>(HlslTree(scene)) },
            },
        };

        #endregion
    }
}
