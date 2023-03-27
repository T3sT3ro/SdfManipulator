using Logic.Nodes.SdfNodes;

namespace Builders.BuiltInTarget.Nodes {
    public class SdfSphereNodeBuilder : NodeBuilder<SdfSphereNode> {
        
        
        public SdfSphereNodeBuilder(ShaderBuilder  builder) : base(builder) { }
        public override string Build(SdfSphereNode input) => $@"";
    }
}
