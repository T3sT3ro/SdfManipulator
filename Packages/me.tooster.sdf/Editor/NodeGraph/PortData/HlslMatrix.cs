using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.Editor.API;
using me.tooster.sdf.Editor.NodeGraph.Nodes;
using UnityEngine;
using Expression = me.tooster.sdf.AST.Syntax.CommonSyntax.Expression<me.tooster.sdf.AST.hlsl>;

namespace me.tooster.sdf.Editor.NodeGraph.PortData {
    public record HlslMatrix(Expression matrixExpression) : Port.Data {
        public static MatrixConstantNode DefaultNode()                => DefaultNode(Matrix4x4.identity);
        public static MatrixConstantNode DefaultNode(Matrix4x4 value) => new MatrixConstantNode(value);
    }
}