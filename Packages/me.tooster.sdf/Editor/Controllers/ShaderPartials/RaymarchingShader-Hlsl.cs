using System;
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST;
using me.tooster.sdf.AST.Hlsl;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor;
using me.tooster.sdf.AST.Hlsl.Syntax.Statements;
using me.tooster.sdf.AST.Syntax;
using me.tooster.sdf.AST.Syntax.CommonSyntax;
using me.tooster.sdf.AST.Syntax.CommonTrivia;
using me.tooster.sdf.Editor.Controllers.Data;
using me.tooster.sdf.Editor.Controllers.SDF;
using me.tooster.sdf.Editor.Util.Controllers;
using Unity.Properties;
using UnityEditor;
using UnityEngine;
using FunctionDefinition = me.tooster.sdf.AST.Hlsl.Syntax.Statements.FunctionDefinition;
using Parameter = me.tooster.sdf.AST.Hlsl.Syntax.Parameter;
using VariableDefinition = me.tooster.sdf.AST.Hlsl.Syntax.Statements.VariableDefinition;
namespace me.tooster.sdf.Editor.Controllers.ShaderPartials {
    public partial class RaymarchingShader {
        IEnumerable<Trivia<hlsl>> pragmasAndIncludes(IEnumerable<string> collectedIncludes) {
            yield return new Pragma { tokenString = "editor_sync_compilation" }.ToStructuredTrivia();
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

        Tree<hlsl> CommonHlslInclude(SdfScene scene) {
            var sceneData = scene.sdfSceneRoot!.sdfData;
            var includes = sceneData.Requirements.OfType<HlslIncludeFileRequirement>()
                .Select(r => r.includeFile);
            var preprocessorHeader = pragmasAndIncludes(includes);
            var globals = generateHlslGlobals(scene).Cast<Statement<hlsl>>();

            return new Tree<hlsl>(globals.ToSyntaxList().WithLeadingTrivia(preprocessorHeader));
        }

        Tree<hlsl> HlslTree(SdfScene scene) {
            var sceneData = scene.sdfSceneRoot!.sdfData;
            var hlslFunctionRequirements = sceneData.Requirements.OfType<HlslFunctionRequirement>();
            var functionRequirements = hlslFunctionRequirements as HlslFunctionRequirement[] ?? hlslFunctionRequirements.ToArray();
            var requiredForwardDeclarations = functionRequirements.Select(r => r.functionDefinition.ForwardDeclaration());
            var requiredFunctions = functionRequirements
                .Select(r => r.functionDefinition);


            return new Tree<hlsl>(
                requiredForwardDeclarations.Cast<Statement<hlsl>>()
                    .Concat(requiredFunctions)
                    .Append(sceneFunctionDefinition(sceneData))
                    .ToSyntaxList()
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
                                    initializer = declaredPropertyType != typeof(Matrix4x4)
                                        ? null
                                        : PropertyContainer.GetValue<Controller, Matrix4x4>(
                                            pd.controller,
                                            pd.path
                                        ).ToPrettyMatrixInitializerList(),
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
                paramList = new Parameter { type = SdfData.pData.typeSyntax, id = SdfData.pParamName },
                body = new Block
                {
                    statements = new Return { expression = sceneData.evaluationExpression(SdfData.pData) },
                },
            };
    }
}
