using System;
using API;
using Logic.Nodes;
using static System.FormattableString;

namespace Builders {
    /// <summary>
    /// Node builder produces 
    /// </summary>
    public abstract class NodeBuilder : Node.Visitor<FormattableString> {
        private ShaderBuilder builder;
        public NodeBuilder(ShaderBuilder builder) => this.builder = builder;

        public virtual string Build(Node node) => Invariant(node.accept(this));

        public FormattableString visit(Node node) => throw new ShaderGenerationException($"Can't handle node '{node}'");
    }
    
}
