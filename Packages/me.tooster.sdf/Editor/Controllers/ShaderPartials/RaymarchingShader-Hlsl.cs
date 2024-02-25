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
using me.tooster.sdf.Editor.Controllers.SDF;
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

        private Tree<hlsl> HlslTree(SdfScene scene) {
            var sceneData = scene.SceneSdfData();

            return new Tree<hlsl>(
                generateHlslGlobals(scene).Cast<Statement<hlsl>>()
                    .Concat(sceneData.requiredFunctionDefinitions)
                    .Append(sceneFunctionDefinition(scene, sceneData))
                    .ToSyntaxList()
                    .WithLeadingTrivia(pragmasAndIncludes(scene.Includes))
            );
        }

        private IEnumerable<VariableDefinition> generateHlslGlobals(
            SdfScene scene) =>
            scene.Properties.SelectMany(group => group.Select(p => {
                try {
                    return new VariableDefinition
                    {
                        declarator = new VariableDeclarator
                        {
                            type = p.hlslTypeToken(),
                            variables = new VariableDeclarator.Definition { id = scene.GetPropertyIdentifier(p) },
                        },
                    };
                } catch (ArgumentOutOfRangeException e) {
                    throw new ArgumentOutOfRangeException($"Unsupported property type '{p.GetType()}' for hlsl global.");
                }
            }));

        private FunctionDefinition sceneFunctionDefinition(SdfScene scene, SdfData sceneData) =>
            new()
            {
                returnType = SdfData.sdfReturnType,
                id = "sdfScene",
                paramList = new FunctionDefinition.Parameter { type = SdfData.sdfArgumentType, id = SdfData.sdfArgumentId },
                body = new Block
                {
                    statements = new[]
                    {
                        new Return
                        {
                            expression = sceneData.evaluationExpression(new VectorData
                            {
                                arity = 3,
                                evaluationExpression = SdfData.sdfArgumentId,
                                vectorType = SdfData.sdfArgumentType.type,
                            }),
                        },
                    },
                },
            };

        #endregion
    }
}
