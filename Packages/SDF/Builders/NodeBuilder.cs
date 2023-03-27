using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using API;

namespace Builders {
    /// <summary>
    /// Builds a node of specified type
    /// </summary>
    /// <typeparam name="NodeT">Node type this builder constructs</typeparam>
    public abstract class NodeBuilder<NodeT> : Builder<string, NodeT> where NodeT : Node {
        protected ShaderBuilder builder;
        public NodeBuilder(ShaderBuilder builder) => this.builder = builder;

        public IEnumerable<Property> DeclaredProperties =>
            this.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(f => f.GetType().IsSubclassOf(typeof(Property)))
                .Select(f => f.GetValue(this) as Property);

        public abstract string Build(NodeT input);
    }
}
