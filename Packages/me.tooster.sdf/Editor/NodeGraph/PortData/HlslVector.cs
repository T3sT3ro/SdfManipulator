using me.tooster.sdf.AST.Hlsl;
using me.tooster.sdf.Editor.API;
using me.tooster.sdf.Editor.NodeGraph.Nodes;
using UnityEngine;

namespace me.tooster.sdf.Editor.NodeGraph.PortData {
    public record HlslVector(Expression vectorExpression) : Port.Data {
        public static VectorConstantNode DefaultNode() => DefaultNode(Vector4.zero);
        public static VectorConstantNode DefaultNode(Vector4 value) => new VectorConstantNode(value);
    }
}