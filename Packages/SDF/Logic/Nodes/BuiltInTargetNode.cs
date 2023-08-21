#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using API;
using AST.Hlsl;
using AST.Shaderlab;
using AST.Shaderlab.Syntax;
using AST.Shaderlab.Syntax.Commands;
using AST.Shaderlab.Syntax.Shader;
using AST.Shaderlab.Syntax.SubShader;
using AST.Syntax;
using UnityEngine;
using static Nodes.Util;
using Property = API.Property;
using PropertySyntax = AST.Shaderlab.Syntax.Shader.Property;
using Shader = AST.Shaderlab.Syntax.Shader.Shader;

namespace Nodes {
    public record BuiltInTargetNode(string targetName) : TargetNode("built_in_target", "Built-in Target") {
        private List<Property>  properties = new();
        private HashSet<string> includes   = new();
        private HashSet<string> defines    = new();

        private void AddProperties(IEnumerable<Property> properties) => this.properties.AddRange(properties);
        private void AddIncludes(ISet<string>            includes)   => this.includes.UnionWith(includes);
        private void AddDefines(ISet<string>             defines)    => this.defines.UnionWith(defines);

        public override string BuildShaderSource() {
            foreach (var node in Graph.TopologicalOrderIterator(this)) {
                AddIncludes(node.CollectIncludes());
                AddDefines(node.CollectDefines());
                AddProperties(node.CollectProperties().Where(v => v.Exposed));
            }

            return shaderTree.ToString();
        }

        private MaterialProperties MaterialProperties => new MaterialProperties
        {
            properties =
                properties.Select(property => property switch
                    {
                        Property<int> p => new PropertySyntax
                        {
                            propertyType = new PredefinedPropertyType { type = new IntegerKeyword() },
                            initializer = new PropertySyntax.Number<IntLiteral> { numberLiteral = p.DefaultValue }
                        },
                        Property<float> p => new PropertySyntax
                        {
                            propertyType = new PredefinedPropertyType { type = new FloatKeyword() },
                            initializer = new PropertySyntax.Number<FloatLiteral> { numberLiteral = p.DefaultValue }
                        },
                        Property<Vector4> p => new PropertySyntax
                        {
                            propertyType = new PredefinedPropertyType { type = new VectorKeyword() },
                            initializer = new PropertySyntax.Vector
                                { arguments = Shaderlab.VectorArgumentList(p.DefaultValue) }
                        },
                        _ => throw new ArgumentOutOfRangeException(nameof(property))
                    } with
                    {
                        id = property.InternalName, displayName = property.DisplayName
                    }).ToList()
        };

        private Tags Tags => new()
        {
            tags = new Tag[]
            {
                new() { key = "RenderType", value = "Opaque" },
                new() { key = "Queue", value = "Geometry+1" },
                new() { key = "IgnoreProjector", value = "True" },
            }
        };

        private IReadOnlyList<SubShaderOrPassStatement> Commands => new List<Command>
        { // TODO: use properties on raymarcher node to set these possibly? 
            new ZTest { operation = new CalculatedArgument { id = "_ZTest" } },
            new Cull { state = new CalculatedArgument { id = "_Cull" } },
            new ZWrite { state = new CalculatedArgument { id = "_ZWrite" } },
        };

        private SubShader SubShader => new SubShader
        {
            statements = new SubShaderOrPassStatement[]
            {
                Tags,
                Pass,
            }.AppendAll(Commands).ToList()
        };

        private Pass Pass => new Pass
        {
            statements = new PassStatement[]
            {
                new HlslProgram { hlslTree = hlslTree }
            }
        };

        private HlslTree hlslTree => null;

        private ShaderlabTree shaderTree => new ShaderlabTree(new Shader
            {
                name = targetName,
                shaderStatements = new ShaderStatement[]
                {
                    MaterialProperties,
                    SubShader,
                }
            }
        );
    }
}
