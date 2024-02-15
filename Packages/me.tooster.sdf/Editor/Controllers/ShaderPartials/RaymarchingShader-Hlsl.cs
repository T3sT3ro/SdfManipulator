using System;
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST;
using me.tooster.sdf.AST.Hlsl;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions;
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators;
using me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor;
using me.tooster.sdf.AST.Hlsl.Syntax.Statements;
using me.tooster.sdf.AST.Hlsl.Syntax.Statements.Definitions;
using me.tooster.sdf.AST.Syntax;
using me.tooster.sdf.AST.Syntax.CommonSyntax;
using me.tooster.sdf.Editor.Controllers.Data;
using UnityEditor;
using static me.tooster.sdf.AST.Hlsl.Extensions;
namespace me.tooster.sdf.Editor.Controllers.ShaderPartials {
    public partial class RaymarchingShader {
        #region hlsl

        private IEnumerable<Trivia<hlsl>> pragmasAndIncludes(IEnumerable<string> collectedIncludes) {
            yield return new Pragma { tokenString = "target 5.0" }.ToStructuredTrivia();
            yield return new Include { filepath = "UnityCG.cginc" }.ToStructuredTrivia();

            var includes = dependentIncludes.Select(AssetDatabase.GetAssetPath)
                .Concat(collectedIncludes)
                .Distinct()
                .ToList();

            foreach (var include in includes)
                yield return new Include { filepath = include }.ToStructuredTrivia();

            yield return new Pragma { tokenString = "vertex vertexShader" }.ToStructuredTrivia();
            yield return new Pragma { tokenString = "fragment fragmentShader" }.ToStructuredTrivia();
        }

        private Tree<hlsl> HlslTree(SdfScene scene) => new(
            generateHlslGlobals(scene).Cast<Statement<hlsl>>()
                .Append(SceneDescription(scene)
                    .requiredFunctionDefinition("sdfScene", SdfPlaceholderScene() /* FIXME: build scene */))
                .ToSyntaxList()
                .WithLeadingTrivia(pragmasAndIncludes(scene.Includes))
        );

        public static Block SdfPlaceholderScene() => new()
        {
            statements = new Statement<hlsl>[]
            {
                (VariableDefinition)new VariableDeclarator
                {
                    type = SdfData.sdfReturnType,
                    variables = new VariableDeclarator.Definition
                    {
                        id = "result",
                        initializer = new Cast
                        {
                            type = SdfData.sdfReturnType,
                            expression = (LiteralExpression)(IntLiteral)0,
                        },
                    },
                },
                Assignment("result.distance", FunctionCall("sdf::primitives3D::sphere", (Identifier)"p", (Identifier)"sphere_1")),
                Assignment("result.id", (LiteralExpression)1),
                (Return)new Identifier { id = "result" },
            },
        };

        private IEnumerable<VariableDefinition> generateHlslGlobals(
            SdfScene scene) =>
            scene.Properties.SelectMany(props => props.Select(p => {
                try {
                    return new VariableDefinition
                    {
                        declarator = new VariableDeclarator
                        {
                            type = p.hlslTypeToken(),
                            variables = new VariableDeclarator.Definition { id = scene.GetIdentifier(p) }.CommaSeparated(),
                        },
                    };
                } catch (ArgumentOutOfRangeException e) {
                    throw new ArgumentOutOfRangeException(
                        $"Unsupported property type '{p.GetType()}' for hlsl global.");
                }
            }));


        private SdfData SceneDescription(SdfScene scene) => new();

        #endregion
    }
}
