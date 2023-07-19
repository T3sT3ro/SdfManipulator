using API;
using Nodes;

namespace BuiltInTarget.Nodes {
    public class VariableNodeBuilder : NodeBuilder<VariableNode> {
        public VariableNodeBuilder(VariableNode node) : base(node) { }

        public static Variable<T>.Evaluator GetEvaluator<T>(VariableNode node) => () => $@"{node.InternalName}";
    }
}