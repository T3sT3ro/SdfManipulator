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
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions;
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators;
using me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor;
using me.tooster.sdf.AST.Hlsl.Syntax.Statements;
using me.tooster.sdf.AST.Hlsl.Syntax.Statements.Declarations;
using me.tooster.sdf.AST.Shaderlab;
using me.tooster.sdf.AST.Shaderlab.Syntax;
using me.tooster.sdf.AST.Shaderlab.Syntax.Commands;
using me.tooster.sdf.AST.Shaderlab.Syntax.SubShaderSpecific;
using me.tooster.sdf.AST.Syntax.CommonSyntax;
using me.tooster.sdf.Editor.NodeGraph.Nodes;
using static me.tooster.sdf.AST.Hlsl.Syntax.VariableDeclarator;
using FloatKeyword = me.tooster.sdf.AST.Shaderlab.Syntax.FloatKeyword;
using FloatLiteral = me.tooster.sdf.AST.Shaderlab.Syntax.FloatLiteral;
using Identifier = me.tooster.sdf.AST.Hlsl.Syntax.Identifier;
using Shader = me.tooster.sdf.AST.Shaderlab.Syntax.Shader;
using MaterialProperties = me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific.MaterialProperties;
using SubShader = me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific.SubShader;
using PropertySyntax = me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific.Property;
using IdentifierToken = me.tooster.sdf.AST.Hlsl.Syntax.IdentifierToken;
using IntLiteral = me.tooster.sdf.AST.Shaderlab.Syntax.IntLiteral;
using Type = me.tooster.sdf.AST.Hlsl.Syntax.Type;

namespace me.tooster.sdf.Editor.Controllers.ShaderPartials {
    public static class RaymarchingShader {
        public static string MainShader(SdfSceneController controller) {
            var formattedSyntax = ShaderlabFormatter.Format(shader(controller));
            return formattedSyntax?.ToString() ?? "// empty shader";
        }

        #region shaderlab

        private static Shader shader(SdfSceneController controller) => new Shader
        {
            name = controller.name + "_g",
            materialProperties = MaterialProperties(controller),
            shaderStatements = new SyntaxList<shaderlab, ShaderStatement>(SubShader)
        };

        /// <summary>
        ///  generates a material properties block from collected properties
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">property type is not supported</exception>
        private static MaterialProperties MaterialProperties(SdfSceneController controller) => new MaterialProperties
        {
            properties =
                controller.Properties
                    .Where(p => p.Exposed)
                    .Select(property => property switch
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
                            id = property.InternalName, displayName = property.DisplayName
                        }).ToList()
        };

        private static TagsBlock TagsBlock => new()
        {
            tags = new[]
            {
                new Tag { key = "RenderType", value = "Opaque" },
                new Tag { key = "Queue", value = "Geometry+1" },
                new Tag { key = "IgnoreProjector", value = "True" },
            }
        };

        private static IEnumerable<SubShaderOrPassStatement> Commands => new List<Command>
        { // TODO: use properties on raymarcher node to set these possibly? 
            new ZTest { operation = new CalculatedArgument { id = "_ZTest" } },
            new Cull { state = new CalculatedArgument { id = "_Cull" } },
            new ZWrite { state = new CalculatedArgument { id = "_ZWrite" } },
        };

        private static SubShader SubShader => new SubShader
        {
            statements = new SyntaxList<shaderlab, SubShaderOrPassStatement>(TagsBlock)
                .Splice(1, 0, Commands)
                .Append<SubShaderOrPassStatement>(Pass)
                .ToList()
        };

        private static Pass Pass => new Pass
        {
            statements = new PassStatement[]
            {
                new HlslProgram { hlsl = new InjectedLanguage<shaderlab, hlsl>(HlslTree) }
            }
        };

        #endregion

        #region hlsl

        #region hlsl - vertex shader

        #endregion

        #region hlsl - fragment shader

        #endregion

        private static Tree<hlsl> HlslTree => new Tree<hlsl>(
            new Statement<hlsl>[]
                {
                    new VariableDeclaration {declarator = new VariableDeclarator
                    {
                        type = new MatrixToken(),
                        variables = new VariableDefinition {id = new Identifier { id = "_BoxPos"}}.CommaSeparated()
                    }}
                    // new FunctionDeclaration 
                }.ToSyntaxList()
                .WithLeadingTrivia(new Include { filepath = "UnityCG.cginc" }.ToStructuredTrivia())
        );

        #endregion
    }
}
