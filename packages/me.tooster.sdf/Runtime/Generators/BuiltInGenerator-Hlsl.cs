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
using me.tooster.sdf.Data;
using me.tooster.sdf.Util;
using Unity.Properties;
using FunctionDefinition = me.tooster.sdf.AST.Hlsl.Syntax.Statements.FunctionDefinition;
using Parameter = me.tooster.sdf.AST.Hlsl.Syntax.Parameter;
using VariableDefinition = me.tooster.sdf.AST.Hlsl.Syntax.Statements.VariableDefinition;
namespace me.tooster.sdf.Generators {
    public partial class BuiltInGenerator {
        IEnumerable<Trivia<hlsl>> includes() {
            // yield return new Pragma { tokenString = "editor_sync_compilation" }.ToStructuredTrivia();
            yield return new Pragma { tokenString = "target 5.0" }.ToStructuredTrivia();
            yield return new Include { filepath = "UnityCG.cginc" }.ToStructuredTrivia();

            // handle simple keyword defines
            foreach (var define in defines)
                yield return new Define { id = define }.ToStructuredTrivia();

            // the passes need their entry points, also when a preset doesn't list the files
            var passIncludes = new[] { SdfPaths.ShaderLibrary.BuiltInShading, SdfPaths.ShaderLibrary.ScenePicking };
            foreach (var include in includeFiles.Concat(passIncludes).Distinct())
                yield return new Include { includeKeyword = new IncludeWithPragmasKeyword(), filepath = include }.ToStructuredTrivia();

            yield return new NewLine<hlsl>();
        }

        /// <summary>The HLSL that every pass shares: includes, uniforms and the scene functions.</summary>
        Tree<hlsl> CommonHlslInclude() {
            var globals = generateHlslGlobals().Cast<Statement<hlsl>>().ToList();
            // generating the scene function collects the other functions as requirements, so it goes first
            var sceneFunction = sceneFunctionDefinition();
            var forwardDeclarations = functionDefinitions.Select(f => f.ForwardDeclaration());

            return new Tree<hlsl>(
                globals
                    .Concat(forwardDeclarations)
                    .Concat(functionDefinitions)
                    .Append(sceneFunction)
                    .ToSyntaxList()
                    .WithLeadingTrivia(includes())
            );
        }

        /// <summary>
        ///     The program of a pass: its pragmas, which select its entry points and keywords, followed by a declaration
        ///     of the scene function, which the shared include block defines.
        /// </summary>
        Tree<hlsl> PassHlslTree(IEnumerable<string> passPragmas) {
            var pragmaTrivia = passPragmas.Distinct()
                .Select<string, Trivia<hlsl>>(p => new Pragma { tokenString = p }.ToStructuredTrivia())
                .Append(new NewLine<hlsl>());
            return new Tree<hlsl>(
                new Statement<hlsl>[] { sceneFunctionDeclaration() }.ToSyntaxList().WithLeadingTrivia(pragmaTrivia)
            );
        }

        /// <summary>
        ///     Declares a uniform for each controller property, without a value: <see cref="SdfScene" /> sets the values on
        ///     its renderers, so changing a value doesn't change the shader (#114).
        /// </summary>
        IEnumerable<VariableDefinition> generateHlslGlobals()
            => scene.sceneData.Properties.Select(pd => {
                    try {
                        var declaredPropertyType = PropertyContainer.GetProperty(pd.controller, pd.path).DeclaredValueType();
                        return new VariableDefinition
                        {
                            declarator = new VariableDeclarator
                            {
                                type = declaredPropertyType.hlslTypeToken(),
                                variables = new VariableDeclarator.Definition { id = pd.identifier },
                            },
                        };
                    } catch (ArgumentOutOfRangeException e) {
                        throw new ArgumentOutOfRangeException($"Unsupported property type '{pd.GetType()}' for hlsl global.", e);
                    }
                }
            );

        static FunctionDeclaration sceneFunctionDeclaration()
            => new()
            {
                returnType = SdfData.sdfReturnType,
                id = "sdfScene",
                paramList = new Parameter { type = SdfData.pData.typeSyntax, id = SdfData.pParamName },
            };

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
