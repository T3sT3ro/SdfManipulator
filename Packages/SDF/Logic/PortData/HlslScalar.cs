using API;
using AST.Hlsl.Syntax.Expressions;
using Nodes;

namespace PortData {
    public record HlslScalar(Expression scalarExpression) : Port.Data {
        public static IntConstantNode   DefaultIntNode()   => new IntConstantNode(0);
        public static FloatConstantNode DefaultFloatNode() => new FloatConstantNode(0f);
    }
}