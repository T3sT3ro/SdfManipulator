using System;
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST;
using me.tooster.sdf.AST.Hlsl;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor;
using me.tooster.sdf.AST.Hlsl.Syntax.Statements;
using me.tooster.sdf.AST.Hlsl.Syntax.Statements.Definitions;
using me.tooster.sdf.AST.Syntax;
using me.tooster.sdf.AST.Syntax.CommonSyntax;
using me.tooster.sdf.AST.Syntax.CommonTrivia;
using me.tooster.sdf.Editor.Controllers.Data;
using me.tooster.sdf.Editor.Controllers.SDF;
using Unity.Properties;
using UnityEditor;
using UnityEngine;
namespace me.tooster.sdf.Editor.Controllers.ShaderPartials {
    public partial class RaymarchingShader {
        IEnumerable<Trivia<hlsl>> pragmasAndIncludes(IEnumerable<string> collectedIncludes) {
            yield return new Pragma { tokenString = "once" }.ToStructuredTrivia();
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
            yield return new NewLine<hlsl>();
        }

        Tree<hlsl> HlslTree(SdfScene scene) {
            var sceneData = scene.sdfSceneRoot!.sdfData;

            return new Tree<hlsl>(
                generateHlslGlobals(scene).Cast<Statement<hlsl>>()
                    .Concat(
                        sceneData.Requirements.OfType<HlslFunctionRequirement>()
                            .Select(r => r.requiredFunction)
                    )
                    .Append(sceneFunctionDefinition(sceneData))
                    .ToSyntaxList()
                    .WithLeadingTrivia(pragmasAndIncludes(scene.Includes))
            );
        }

        IEnumerable<VariableDefinition> generateHlslGlobals(SdfScene scene)
            => scene.sceneData.Properties.Select(
                pd => {
                    try {
                        var declaredPropertyType = PropertyContainer.GetProperty(pd.controller, pd.path).DeclaredValueType();
                        return new VariableDefinition
                        {
                            declarator = new VariableDeclarator
                            {
                                type = declaredPropertyType.hlslTypeToken(),
                                variables = new VariableDeclarator.Definition
                                {
                                    id = pd.identifier,
                                    initializer = declaredPropertyType != typeof(Matrix4x4) ? null : (Identifier)"MATRIX_ID",
                                },
                            },
                        };
                    } catch (ArgumentOutOfRangeException e) {
                        throw new ArgumentOutOfRangeException($"Unsupported property type '{pd.GetType()}' for hlsl global.", e);
                    }
                }
            );

        FunctionDefinition sceneFunctionDefinition(SdfData sceneData)
            => new()
            {
                returnType = SdfData.sdfReturnType,
                id = "sdfScene",
                paramList = new FunctionDefinition.Parameter { type = SdfData.pData.typeSyntax, id = SdfData.pParamName },
                body = new Block
                {
                    statements = new Return { expression = sceneData.evaluationExpression(SdfData.pData) },
                },
            };
    }
}
