using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.Editor.API;
using me.tooster.sdf.Editor.NodeGraph.Nodes;
using Expression = me.tooster.sdf.AST.Syntax.CommonSyntax.Expression<me.tooster.sdf.AST.hlsl>;

namespace me.tooster.sdf.Editor.NodeGraph.PortData {
    public record HlslScalar(Expression scalarExpression) : Data {
        public static IntConstantNode   DefaultIntNode(int     value = 0) => new IntConstantNode(value);
        public static FloatConstantNode DefaultFloatNode(float value = 0) => new FloatConstantNode(value);
    }
}
