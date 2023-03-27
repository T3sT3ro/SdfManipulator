using System.Linq;
using Logic.Nodes;

namespace Builders.BuiltInTarget.Nodes {
    public class PropertyNodeBuilder : NodeBuilder<PropertyNode> {
        public PropertyNodeBuilder(ShaderBuilder builder) : base(builder) { }
        
        public override string Build(PropertyNode node) => $@"{node.InternalName}";
    }
}
