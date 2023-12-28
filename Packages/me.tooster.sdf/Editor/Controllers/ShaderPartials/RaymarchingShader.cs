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
using me.tooster.sdf.AST.Hlsl.Syntax.Trivias;
using me.tooster.sdf.AST.Shaderlab;
using me.tooster.sdf.AST.Shaderlab.Syntax;
using me.tooster.sdf.AST.Shaderlab.Syntax.Commands;
using me.tooster.sdf.AST.Shaderlab.Syntax.SubShaderSpecific;
using me.tooster.sdf.Editor.NodeGraph.Nodes;
using static me.tooster.sdf.AST.Hlsl.Syntax.VariableDeclarator;
using FloatKeyword = me.tooster.sdf.AST.Shaderlab.Syntax.FloatKeyword;
using FloatLiteral = me.tooster.sdf.AST.Shaderlab.Syntax.FloatLiteral;
using Shader = me.tooster.sdf.AST.Shaderlab.Syntax.Shader;
using MaterialProperties = me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific.MaterialProperties;
using SubShader = me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific.SubShader;
using PropertySyntax = me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific.Property;
using HlslStatement = me.tooster.sdf.AST.Hlsl.Syntax.Statement;
using IdentifierToken = me.tooster.sdf.AST.Hlsl.Syntax.IdentifierToken;
using IntLiteral = me.tooster.sdf.AST.Shaderlab.Syntax.IntLiteral;
using Type = me.tooster.sdf.AST.Hlsl.Syntax.Type;

namespace me.tooster.sdf.Editor.Controllers.ShaderPartials {
    public abstract class RaymarchingShader : ScriptableObject {
        public string MainShader(SdfSceneController controller) {
            return ShaderlabFormatter.Format(shader(controller))?.ToString() ?? "// generation failed";
        }

        #region shaderlab

        private Shader shader(SdfSceneController controller) => new Shader
        {
            name = controller.name + "_g",
            materialProperties = MaterialProperties(controller),
            shaderStatements = new SyntaxList<shaderlab, ShaderStatement>(SubShader)
        };

        /// <summary>
        ///  generates a material properties block from collected properties
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">property type is not supported</exception>
        private MaterialProperties MaterialProperties(SdfSceneController controller) => new MaterialProperties
        {
            properties =
                controller.Properties.Select(property => property switch
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
                            "Material property block can't support this property type")
                    } with
                    {
                        id = property.InternalName, displayName = property.DisplayName
                    }).ToList()
        };

        private TagsBlock TagsBlock => new()
        {
            tags = new Tag[]
            {
                new() { key = "RenderType", value = "Opaque" },
                new() { key = "Queue", value = "Geometry+1" },
                new() { key = "IgnoreProjector", value = "True" },
            }
        };

        private IEnumerable<SubShaderOrPassStatement> Commands => new List<Command>
        { // TODO: use properties on raymarcher node to set these possibly? 
            new ZTest { operation = new CalculatedArgument { id = "_ZTest" } },
            new Cull { state = new CalculatedArgument { id = "_Cull" } },
            new ZWrite { state = new CalculatedArgument { id = "_ZWrite" } },
        };

        private SubShader SubShader => new SubShader
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

        #region hlsl - definitions

        private const string positionMemberName = "pos";
        private const string normalMemberName   = "normal";
        private const string uvMemberName       = "uv";
        private const string hitposMemberName   = "hitpos";
        private const string rdCamMemberName    = "rd_cam";

        private static Member PositionMemberAccess => new Member { member = positionMemberName };
        private static Member NormalMemberAccess   => new Member { member = normalMemberName };
        private static Member UVMemberAccess       => new Member { member = uvMemberName };
        private static Member HitposMemberAccess   => new Member { member = hitposMemberName };
        private static Member RDCamMemberAccess    => new Member { member = rdCamMemberName };

        // struct v_in { float4 pos : SV_POSITION; ... };
        public static Type.Struct VertexInputStruct => new Type.Struct
        {
            name = "vertex",
            members = new[]
            {
                new Type.Struct.Member
                {
                    type = new VectorToken { arity = 4, type = new AST.Hlsl.Syntax.FloatKeyword() },
                    id = positionMemberName,
                    semantic = new PositionSemantic()
                },
                new Type.Struct.Member
                {
                    type = new VectorToken { arity = 3, type = new AST.Hlsl.Syntax.FloatKeyword() },
                    id = normalMemberName,
                    semantic = new NormalSemantic()
                },
                new Type.Struct.Member
                {
                    type = new VectorToken { arity = 2, type = new AST.Hlsl.Syntax.FloatKeyword() },
                    id = uvMemberName,
                    semantic = new TexcoordSemantic { n = 0 }
                },
                new Type.Struct.Member
                {
                    type = new VectorToken { arity = 3, type = new AST.Hlsl.Syntax.FloatKeyword() },
                    id = hitposMemberName,
                    semantic = new TexcoordSemantic { n = 1 }
                },
                new Type.Struct.Member
                {
                    type = new VectorToken { arity = 3, type = new AST.Hlsl.Syntax.FloatKeyword() },
                    id = rdCamMemberName,
                    semantic = new TexcoordSemantic { n = 2 }
                }
            }
        };
        
        

        #endregion

        #region hlsl - vertex shader

        #endregion

        #region hlsl - fragment shader

        #endregion

        #endregion

        private static Tree<hlsl> HlslTree => new Tree<hlsl>(
            new HlslStatement[]
            {
                new StructDeclaration { shape = VertexInputStruct },
                new StructDeclaration { shape = VertexToFragmentStruct },
                new FunctionDeclaration 
            }.ToSyntaxList()
        );

        private const string vertexMethodName    = "vert";
        private const string fragmentMethodName  = "frag";
        private const string vertexInParamName   = "v";
        private const string vertexOutVarName    = "output";
        private const string fragmentInParamName = "fragment";


        private FunctionDeclaration vertFunctionDeclaration => new FunctionDeclaration
        {
            id = new IdentifierToken
            {
                ValidatedText = vertexMethodName,
                LeadingTriviaList = new Trivia<hlsl>[]
                {
                    new PreprocessorDirective { Structure = new Pragma { tokenString = $"vertex {vertexMethodName}" } },
                    new PreprocessorDirective
                        { Structure = new Pragma { tokenString = $"fragment {fragmentMethodName}" } },
                }
            },
            paramList = new FunctionDeclaration.Parameter[]
            {
                new()
                {
                    type = new Type.UserDefined { id = vertexInputNode.VertexInputStruct.name! },
                    id = vertexInParamName
                }
            },
            body = new Block { statements = vertFunctionBody }
        };

        private Statement assign(string left, Expression right) => new ExpressionStatement
        {
            expression = new AssignmentExpression
            {
                left = left.Split('.').Aggregate((Expression)new Identifier { id = left.Split('.')[0] },
                    (acc, member) => new Member { expression = acc, member = member }),
                right = right
            }
        };

        private SyntaxList<hlsl, HlslStatement> vertFunctionBody => new HlslStatement[]
        {
            new VariableDeclaration
            {
                declarator = new VariableDeclarator
                {
                    type = new Type.UserDefined { id = vertexToFragmentNode.VertexToFragmentStruct.name! },
                    variables = new[] { new VariableDeclarator.VariableDefinition { id = vertexOutVarName } }
                        .CommaSeparated()
                },
            },
            new Return { expression = new Identifier { id = "v2f" } }
        }.ToSyntaxList();
    }
}
