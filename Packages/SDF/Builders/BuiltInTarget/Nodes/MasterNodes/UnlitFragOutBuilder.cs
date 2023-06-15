using API;
using Nodes.MasterNodes;

namespace Builders.BuiltInTarget.Nodes.MasterNodes {
    public class UnlitFragmentNodeBuilder : NodeBuilder<FragOutNode> {
        public UnlitFragmentNodeBuilder(FragOutNode node) : base(node) { }

        public UnlitFragOutNode.Evaluator evaluator = color => color();
    }
}
