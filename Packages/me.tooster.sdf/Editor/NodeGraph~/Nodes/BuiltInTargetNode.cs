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
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators;
using me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor;
using me.tooster.sdf.AST.Hlsl.Syntax.Statements;
using me.tooster.sdf.AST.Hlsl.Syntax.Statements.Declarations;
using me.tooster.sdf.AST.Hlsl.Syntax.Statements.Definitions;
using me.tooster.sdf.AST.Shaderlab;
using me.tooster.sdf.AST.Shaderlab.Syntax;
using me.tooster.sdf.AST.Shaderlab.Syntax.Commands;
using me.tooster.sdf.AST.Shaderlab.Syntax.SubShaderSpecific;
using me.tooster.sdf.AST.Syntax.CommonSyntax;
using me.tooster.sdf.Editor.API.Graph;
using me.tooster.sdf.Editor.Util;

using Unity.VisualScripting.YamlDotNet.Serialization.NamingConventions;
using static me.tooster.sdf.AST.Hlsl.Syntax.VariableDeclarator;
using Extensions = me.tooster.sdf.Editor.API.Extensions;
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
using VariableDefinition = me.tooster.sdf.AST.Hlsl.Syntax.Statements.Definitions.VariableDefinition;

namespace me.tooster.sdf.Editor.NodeGraph.Nodes {
    #region 3 integral nodes for built-in pipeline

    public interface IVertexInputNode {
        Type.Struct VertexInputStruct { get; }
    }

    public interface IVertexToFragmentNode {
        Type.Struct VertexToFragmentStruct { get; }
    }

    public interface IFragmentOutNode {
        Type.Struct FragmentOutStruct { get; }
    }

    #endregion

    /**
     * A target node for the built-in pipeline.
     * FIXME: extract as dependency on other 3 master nodes
     */
    public record BuiltInTargetNode(
        string targetName,
        IVertexInputNode vertexInputNode,
        IVertexToFragmentNode vertexToFragmentNode,
        IFragmentOutNode fragmentOutNode)
        : TargetNode($"{Extensions.sanitizeNameToIdentifier(targetName)}_target", $"{targetName} Target") {
        private List<Property>  properties = new();
        private HashSet<string> includes   = new();
        private HashSet<string> defines    = new();

        private void AddProperties(IEnumerable<Property> properties) => this.properties.AddRange(properties);
        private void AddIncludes(IEnumerable<string> includes)       => this.includes.UnionWith(includes);
        private void AddDefines(IEnumerable<string> defines)         => this.defines.UnionWith(defines);

        public override string BuildShaderSource() => BuildShaderSyntaxTree().ToString();

        private Tree<shaderlab> BuildShaderSyntaxTree() {
            foreach (var node in Graph.NodeTopologicalIterator(this)) {
                /*AddIncludes(Controller.CollectIncludes(node));
                AddDefines(CollectDefines(node));
                AddProperties(CollectProperties(node).Where(v => v.Exposed));*/
            }

            return new Tree<shaderlab>(
                Root: shaderTree.Root is null ? null : ShaderlabFormatter.Format(shaderTree.Root));
        }
        
        private Tree<shaderlab> shaderTree => new Tree<shaderlab>(new Shader
            {
                name = targetName,
                materialProperties = MaterialProperties,
                shaderStatements = new SyntaxList<shaderlab, ShaderStatement>(SubShader)
            }
        );

        /// <summary>
        ///  generates a material properties block from collected properties
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">property type is not supported</exception>
        private MaterialProperties MaterialProperties => new MaterialProperties
        {
            properties =
                properties.Select(property => property switch
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
                                { arguments = p.DefaultValue.VectorArgumentList() }
                        },
                        _ => throw new ArgumentOutOfRangeException(nameof(property),
                            "Material property block can't support this property type")
                    } with
                    {
                        id = Graph.GetUniqueNameForProperty(property), displayName = property.DisplayName
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

        private static Tree<hlsl> HlslTree => new Tree<hlsl>(
            new Statement<hlsl>[]
            {
                new FunctionDefinition { }
            }.ToSyntaxList()
        );

        private SyntaxList<hlsl, StructDefinition> declaredStructs => new SyntaxList<hlsl, StructDefinition>(
            vertexInputNode.VertexInputStruct,
            vertexToFragmentNode.VertexToFragmentStruct,
            fragmentOutNode.FragmentOutStruct
        );

        private const string vertexMethodName    = "vert";
        private const string fragmentMethodName  = "frag";
        private const string vertexInParamName   = "v";
        private const string vertexOutVarName    = "output";
        private const string fragmentInParamName = "fragment";


        private FunctionDefinition VertFunctionDefinition => new FunctionDefinition
        {
            id = new IdentifierToken { ValidatedText = vertexMethodName, },
            paramList = new FunctionDefinition.Parameter[]
            {
                new()
                {
                    type = new Type.UserDefined { id = vertexInputNode.VertexInputStruct.name! },
                    id = vertexInParamName
                }
            },
            body = new Block { statements = vertFunctionBody }
        };

        private Statement<hlsl> assign(string left, Expression<hlsl> right) => new ExpressionStatement
        {
            expression = new AssignmentExpression
            {
                left = left.Split('.').Aggregate((Expression<hlsl>)new Identifier { id = left.Split('.')[0] },
                    (acc, member) => new Member { expression = acc, member = member }),
                right = right
            }
        };

        private SyntaxList<hlsl, Statement<hlsl>> vertFunctionBody => new Statement<hlsl>[]
        {
            new VariableDefinition
            {
                declarator = new VariableDeclarator
                {
                    type = new Type.UserDefined { id = vertexToFragmentNode.VertexToFragmentStruct.name! },
                    variables = new[] { new VariableDeclarator.VariableDefinition { id = vertexOutVarName } }.CommaSeparated()
                },
            },
            new Return { expression = new Identifier { id = "v2f" } }
        }.ToSyntaxList();
    }
}
