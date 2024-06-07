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
using UnityEngine;
using FunctionDefinition = me.tooster.sdf.AST.Hlsl.Syntax.Statements.FunctionDefinition;
using Parameter = me.tooster.sdf.AST.Hlsl.Syntax.Parameter;
using VariableDefinition = me.tooster.sdf.AST.Hlsl.Syntax.Statements.VariableDefinition;
namespace me.tooster.sdf.Editor.Controllers.Generators {
    public partial class BuiltInGenerator {
        IEnumerable<Trivia<hlsl>> pragmasAndIncludes() {
            // yield return new Pragma { tokenString = "editor_sync_compilation" }.ToStructuredTrivia();
            yield return new Pragma { tokenString = "target 5.0" }.ToStructuredTrivia();
            yield return new Include { filepath = "UnityCG.cginc" }.ToStructuredTrivia();

            foreach (var include in includeFiles)
                yield return new Include { filepath = include }.ToStructuredTrivia();

            yield return new Pragma { tokenString = "vertex vertexShader" }.ToStructuredTrivia();
            yield return new Pragma { tokenString = "fragment fragmentShader" }.ToStructuredTrivia();
            yield return new NewLine<hlsl>();
        }

        Tree<hlsl> CommonHlslInclude() {
            var preprocessorHeader = pragmasAndIncludes();
            var globals = generateHlslGlobals().Cast<Statement<hlsl>>();

            return new Tree<hlsl>(globals.ToSyntaxList().WithLeadingTrivia(preprocessorHeader));
        }

        Tree<hlsl> HlslTree() {
            var forwardDeclarations = functionDefinitions.Select(f => f.ForwardDeclaration());

            return new Tree<hlsl>(
                forwardDeclarations.Cast<Statement<hlsl>>()
                    .Concat(functionDefinitions)
                    .Append(sceneFunctionDefinition())
                    .ToSyntaxList()
            );
        }

        IEnumerable<VariableDefinition> generateHlslGlobals()
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

        FunctionDefinition sceneFunctionDefinition()
            => new()
            {
                returnType = SdfData.sdfReturnType,
                id = "sdfScene",
                paramList = new Parameter { type = SdfData.pData.typeSyntax, id = SdfData.pParamName },
                body = new Block
                {
                    statements = new Return { expression = scene.sdfSceneRoot.Apply(SdfData.pData, this).evaluationExpression },
                },
            };
    }
}
