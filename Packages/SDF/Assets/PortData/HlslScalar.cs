using Assets.Nodes;
using AST.Hlsl;

namespace PortData {
    public record HlslScalar(Expression scalarExpression) : API.Port.Data {
        public static IntConstantNode   DefaultIntNode(int     value = 0) => new IntConstantNode(value);
        public static FloatConstantNode DefaultFloatNode(float value = 0) => new FloatConstantNode(value);
    }
}
