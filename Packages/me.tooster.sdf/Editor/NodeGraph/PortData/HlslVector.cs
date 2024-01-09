using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.Editor.API;
using me.tooster.sdf.Editor.NodeGraph.Nodes;
using UnityEngine;
using Expression = me.tooster.sdf.AST.Syntax.CommonSyntax.Expression<me.tooster.sdf.AST.hlsl>;

namespace me.tooster.sdf.Editor.NodeGraph.PortData {
    public record HlslVector(Expression vectorExpression) : Port.Data {
        public static VectorConstantNode DefaultNode() => DefaultNode(Vector4.zero);
        public static VectorConstantNode DefaultNode(Vector4 value) => new VectorConstantNode(value);
    }
}