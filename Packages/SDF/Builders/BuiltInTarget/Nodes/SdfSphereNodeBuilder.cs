using API;
using Nodes;
using Nodes.SdfNodes;

namespace Builders.BuiltInTarget.Nodes {
    public class SdfSphereNodeBuilder : NodeBuilder<SdfNode> {
        public SdfSphereNodeBuilder(SdfNode node) : base(node) { }

        public delegate SdfNode.Evaluator SdfSphereNodeEvaluator(SdfSphereNode node);

        public static string Build(SdfSphereNode node) { return $"sdf_sphere({node.Center}, {node.Radius})"; }
    }
}
