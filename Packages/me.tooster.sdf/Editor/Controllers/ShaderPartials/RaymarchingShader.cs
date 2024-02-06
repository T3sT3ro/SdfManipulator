#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using me.tooster.sdf.Editor.API;
using me.tooster.sdf.AST;
using me.tooster.sdf.AST.Syntax;
using me.tooster.sdf.AST.Hlsl;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor;
using me.tooster.sdf.AST.Hlsl.Syntax.Statements.Declarations;
using me.tooster.sdf.AST.Shaderlab;
using me.tooster.sdf.AST.Shaderlab.Syntax;
using me.tooster.sdf.AST.Shaderlab.Syntax.Commands;
using me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific;
using me.tooster.sdf.AST.Shaderlab.Syntax.SubShaderSpecific;
using me.tooster.sdf.AST.Syntax.CommonSyntax;
using me.tooster.sdf.Editor.NodeGraph.Nodes;
using UnityEditor;
using static me.tooster.sdf.AST.Hlsl.Syntax.VariableDeclarator;
using Attribute = me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific.Attribute;
using BooleanLiteral = me.tooster.sdf.AST.Shaderlab.Syntax.BooleanLiteral;
using Constants = me.tooster.sdf.AST.Hlsl.Constants;
using Extensions = me.tooster.sdf.AST.Syntax.Extensions;
using FloatKeyword = me.tooster.sdf.AST.Shaderlab.Syntax.FloatKeyword;
using FloatLiteral = me.tooster.sdf.AST.Shaderlab.Syntax.FloatLiteral;
using Identifier = me.tooster.sdf.AST.Hlsl.Syntax.Identifier;
using Shader = me.tooster.sdf.AST.Shaderlab.Syntax.Shader;
using MaterialProperties = me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific.MaterialProperties;
using SubShader = me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific.SubShader;
using PropertySyntax = me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific.Property;
using IntLiteral = me.tooster.sdf.AST.Shaderlab.Syntax.IntLiteral;
using LiteralExpression = me.tooster.sdf.AST.Hlsl.Syntax.Expressions.LiteralExpression;
using Property = me.tooster.sdf.Editor.API.Property;
using Type = System.Type;

namespace me.tooster.sdf.Editor.Controllers.ShaderPartials {
    // TODO: refactor into builder + SO and cache partially generated syntax
    public class RaymarchingShader : ScriptableSingleton<RaymarchingShader> {
        public ShaderInclude[] dependentIncludes = null!;

        private RaymarchingShader() { }

        private void OnEnable() {
            dependentIncludes = new[]
            {
                "Packages/me.tooster.sdf/Editor/Resources/Includes/types.hlsl",
                "Packages/me.tooster.sdf/Editor/Resources/Includes/util.hlsl",
                "Packages/me.tooster.sdf/Editor/Resources/Includes/matrix.hlsl",
                "Packages/me.tooster.sdf/Editor/Resources/Includes/primitives.hlsl",
                "Packages/me.tooster.sdf/Editor/Resources/Includes/operators.hlsl",
                "Packages/me.tooster.sdf/Editor/Resources/Includes/noise.hlsl",
                "Packages/me.tooster.sdf/Editor/Resources/Includes/raymarching.hlsl",
            }.Select(AssetDatabase.LoadAssetAtPath<ShaderInclude>).ToArray();
        }

        public string MainShader(SdfScene scene) {
            var unformatedSource = shader(scene);
            var formattedSource = ShaderlabFormatter.Format(unformatedSource);
            return formattedSource?.ToString() ?? "// empty shader";
        }

        #region shaderlab

        private Shader shader(SdfScene scene) => new Shader
        {
            name = scene.name + "_g",
            materialProperties = MaterialProperties(scene),
            shaderStatements = new ShaderStatement[]
            {
                new Fallback { name = "Sdf/Fallback"},
                SubShader(scene),
            }
        };

        /// <summary>
        ///  generates a material properties block from collected properties
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">property type is not supported</exception>
        private MaterialProperties MaterialProperties(SdfScene scene) {
            return new MaterialProperties
            {
                properties = GlobalShaderProperties
                    .Concat(scene.Properties.SelectMany(properties =>
                            properties.Select(p => generatePropertySyntax(scene, p)))
                        .Select((p, i) => i > 0
                            ? p
                            : p with
                            {
                                attributes = new[]
                                {
                                    SyntaxExtensions.headerAttribute("raymarching properties"),
                                    SyntaxExtensions.spaceAttribute(),
                                }
                            }))
                    .ToList()
            };
        }

        private static IEnumerable<PropertySyntax> GlobalShaderProperties {
            get {
                return new[]
                {
                    // [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull", Int) = 0
                    new PropertySyntax
                    {
                        id = "_Cull", displayName = "Cull", propertyType = new IntegerKeyword(),
                        initializer = new PropertySyntax.Number<IntLiteral> { numberLiteral = 0 },
                        attributes = new[]
                        {
                            SyntaxExtensions.headerAttribute("global raymarching properties"),
                            SyntaxExtensions.spaceAttribute(),
                            new Attribute
                            {
                                id = "Enum",
                                arguments = new Attribute.Value { value = "UnityEngine.Rendering.CullMode" }
                            },
                        }
                    },
                    // [Tooltip(Enable to assure correct blending of multiple domains and backface rendering)]
                    // [Toggle][KeyEnum(Off, On)] _ZWrite ("ZWrite", Float) = 0
                    new PropertySyntax
                    {
                        id = "_ZWrite", displayName = "ZWrite", propertyType = new FloatKeyword(),
                        initializer = new PropertySyntax.Number<IntLiteral> { numberLiteral = 0 },
                        attributes = new[]
                        {
                            new Attribute
                            {
                                id = "Tooltip",
                                arguments = new Attribute.Value
                                {
                                    value =
                                        "Enable to assure correct blending of multiple domains and backface rendering"
                                }
                            },
                            new Attribute { id = "Toggle" },
                            new Attribute
                            {
                                id = "KeyEnum",
                                arguments = new Attribute.Value[] { "Off", "On" }
                            }
                        }
                    },
                    // [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Int) = 1
                    new PropertySyntax
                    {
                        id = "_ZTest", displayName = "ZTest", propertyType = new IntegerKeyword(),
                        initializer = new PropertySyntax.Number<IntLiteral> { numberLiteral = 1 },
                        attributes = new Attribute
                        {
                            id = "Enum",
                            arguments = new Attribute.Value { value = "UnityEngine.Rendering.CompareFunction" }
                        }
                    }
                };
            }
        }

        private PropertySyntax generatePropertySyntax(IPropertyIdentifierProvider scene, Property property) =>
            property switch
                {
                    Property<int> p => new PropertySyntax
                    {
                        propertyType = new PropertySyntax.PredefinedType { type = new IntegerKeyword() },
                        initializer = new PropertySyntax.Number<IntLiteral> { numberLiteral = p.DefaultValue }
                    },
                    Property<float> p => new PropertySyntax
                    {
                        propertyType = new PropertySyntax.PredefinedType { type = new FloatKeyword() },
                        initializer = new PropertySyntax.Number<FloatLiteral> { numberLiteral = p.DefaultValue }
                    },
                    Property<Vector4> p => new PropertySyntax
                    {
                        propertyType = new PropertySyntax.PredefinedType { type = new VectorKeyword() },
                        initializer = new PropertySyntax.Vector
                            { arguments = ShaderlabUtil.VectorArgumentList(p.DefaultValue) }
                    },
                    _ => throw new ArgumentOutOfRangeException(nameof(property),
                        $"Material property block can't expose property of type {property.GetType()}")
                } with
                {
                    id = scene.GetIdentifier(property),
                    displayName = property.DisplayName,
                };

        private static TagsBlock TagsBlock => new()
        {
            tags = new[]
            {
                new Tag { key = "RenderType", value = "Opaque" },
                new Tag { key = "Queue", value = "Geometry+1" },
                new Tag { key = "IgnoreProjector", value = "True" },
                new Tag { key = "LightMode", value = "ForwardBase" },
            }
        };

        private IEnumerable<SubShaderOrPassStatement> Commands => new List<Command>
        { // TODO: use properties on raymarcher node to set these possibly? 
            new ZTest { operation = new CalculatedArgument { id = "_ZTest" } },
            new Cull { state = new CalculatedArgument { id = "_Cull" } },
            new ZWrite { state = new CalculatedArgument { id = "_ZWrite" } },
        };

        private SubShader SubShader(SdfScene scene) => new SubShader
        {
            statements = new SyntaxList<shaderlab, SubShaderOrPassStatement>(TagsBlock)
                .Splice(1, 0, Commands)
                .Append<SubShaderOrPassStatement>(Pass(scene))
                .ToList()
        };

        private Pass Pass(SdfScene scene) => new Pass
        {
            statements = new PassStatement[]
            {
                new HlslProgram { hlsl = new InjectedLanguage<shaderlab, hlsl>(HlslTree(scene)) }
            }
        };

        #endregion

        #region hlsl

        private IEnumerable<Trivia<hlsl>> pragmasAndIncludes() {
            yield return new Pragma { tokenString = "target 5.0" }.ToStructuredTrivia();
            yield return new Include { filepath = "UnityCG.cginc" }.ToStructuredTrivia();

            foreach (var include in dependentIncludes)
                yield return new Include { filepath = AssetDatabase.GetAssetPath(include) }.ToStructuredTrivia();

            yield return new Pragma { tokenString = "vertex vertexShader" }.ToStructuredTrivia();
            yield return new Pragma { tokenString = "fragment fragmentShader" }.ToStructuredTrivia();
        }

        private Tree<hlsl> HlslTree(SdfScene scene) => new Tree<hlsl>(
            generateHlslGlobals(scene)
                .ToSyntaxList()
                .WithLeadingTrivia(pragmasAndIncludes())
        );

        private IEnumerable<VariableDeclaration> generateHlslGlobals(SdfScene scene) =>
            scene.Properties.SelectMany(props => props.Select(p => {
                try {
                    return new VariableDeclaration
                    {
                        declarator = new VariableDeclarator
                        {
                            type = p.hlslTypeToken(),
                            variables = new VariableDefinition { id = scene.GetIdentifier(p) }.CommaSeparated()
                        }
                    };
                } catch (ArgumentOutOfRangeException e) {
                    throw new ArgumentOutOfRangeException(
                        $"Unsupported property type '{p.GetType()}' for hlsl global.");
                }
            }));

        #endregion
    }
}
