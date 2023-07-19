using API;
using Nodes.MasterNodes;
using UnityEngine;

namespace BuiltInTarget.Nodes.MasterNodes {
    public class UnlitFragmentNodeBuilder : NodeBuilder<UnlitFragOutNode> {
        public UnlitFragmentNodeBuilder(UnlitFragOutNode node) : base(node) { }

        [ShaderGlobal]
        private static string fragOutStruct = $@"
struct f2p {{
    fixed4 {nameof(UnlitFragOutNode.color).ToLower()}: SV_Target0;
}};
";

        public override NodeBuilder.Selector GetSelectorFor(InputPort port) {
            if (port == node.color) return (value) => $"{value}.{nameof(UnlitFragOutNode.color).ToLower()}";
            else return value => "";
        }

        public FragOutNode.Evaluator GegtEvaluator(Variable<Vector4>.Evaluator fragColor) => () => $@"
f2p output;
output.color = {fragColor()};
return output;
";
    }
}
