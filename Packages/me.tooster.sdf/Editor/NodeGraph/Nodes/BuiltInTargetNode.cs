#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Hlsl;
using me.tooster.sdf.AST.Shaderlab;
using me.tooster.sdf.AST.Shaderlab.Syntax;
using me.tooster.sdf.AST.Shaderlab.Syntax.Commands;
using me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific;
using me.tooster.sdf.AST.Shaderlab.Syntax.SubShaderSpecific;
using me.tooster.sdf.AST.Syntax;
using me.tooster.sdf.Editor.API;
using UnityEngine;
using FloatKeyword = me.tooster.sdf.AST.Shaderlab.Syntax.FloatKeyword;
using FloatLiteral = me.tooster.sdf.AST.Shaderlab.Syntax.FloatLiteral;
using IntLiteral = me.tooster.sdf.AST.Shaderlab.Syntax.IntLiteral;
using Property = me.tooster.sdf.Editor.API.Property;
using PropertySyntax = me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific.Property;
using Shader = me.tooster.sdf.AST.Shaderlab.Syntax.Shader;
using Shaderlab = me.tooster.sdf.AST.Shaderlab.Shaderlab;

namespace me.tooster.sdf.Editor.NodeGraph.Nodes {
    public record BuiltInTargetNode(string targetName)
        : TargetNode($"{Representable.formatter.Apply(targetName)}_target", $"{targetName} Target") {
        private List<Property>  properties = new();
        private HashSet<string> includes   = new();
        private HashSet<string> defines    = new();

        private void AddProperties(IEnumerable<Property> properties) => this.properties.AddRange(properties);
        private void AddIncludes(IEnumerable<string>     includes)   => this.includes.UnionWith(includes);
        private void AddDefines(IEnumerable<string>      defines)    => this.defines.UnionWith(defines);


        public override ITree BuildShaderSyntaxTree() {
            foreach (var node in Graph.NodeTopologicalIterator(this)) {
                AddIncludes(node.CollectIncludes());
                AddDefines(node.CollectDefines());
                AddProperties(node.CollectProperties().Where(v => v.Exposed));
            }

            return new Tree<Shaderlab>(Root: ShaderlabFormatter.Format(shaderTree.Root));
        }

        private Tree<Shaderlab> shaderTree => new Tree<Shaderlab>(new Shader
            {
                name = targetName,
                shaderStatements = new ShaderStatement[]
                {
                    MaterialProperties,
                    SubShader,
                }
            }
        );

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
                                { arguments = ShaderlabUtil.VectorArgumentList(p.DefaultValue) }
                        },
                        _ => throw new ArgumentOutOfRangeException(nameof(property))
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
            statements = new SubShaderOrPassStatement[]
            {
                TagsBlock,
                Pass,
            }.AppendAll(Commands).ToList()
        };

        private Pass Pass => new Pass
        {
            statements = new PassStatement[]
            {
                new HlslProgram { hlsl = new InjectedLanguage<Shaderlab, Hlsl>(hlslTree) }
            }
        };

        private Tree<Hlsl> hlslTree => new Tree<Hlsl>(
            new Statement[]
            {
                
            }.ToSyntaxList()
        );
    }
}
