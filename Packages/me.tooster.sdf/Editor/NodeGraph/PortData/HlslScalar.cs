using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.Editor.API;
using me.tooster.sdf.Editor.NodeGraph.Nodes;

namespace me.tooster.sdf.Editor.NodeGraph.PortData {
    public record HlslScalar(Expression scalarExpression) : Port.Data {
        public static IntConstantNode   DefaultIntNode(int     value = 0) => new IntConstantNode(value);
        public static FloatConstantNode DefaultFloatNode(float value = 0) => new FloatConstantNode(value);
    }
}
