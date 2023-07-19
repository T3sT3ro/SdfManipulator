using API;
using Nodes;
using Nodes.SdfNodes;

namespace BuiltInTarget.Nodes {
    [ShaderInclude("Packages/SDF/Logic/Includes/primitives.cginc")]
    public class SdfSphereNodeBuilder : NodeBuilder<SdfNode> {
        public SdfSphereNodeBuilder(SdfNode node) : base(node) { }
        
        public static string GetEvaluator(SdfSphereNode node) { return $"sdf_sphere({node.center}, {node.radius})"; }
    }
}
