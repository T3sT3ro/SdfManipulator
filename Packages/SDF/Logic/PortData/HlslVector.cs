using API;
using AST.Hlsl.Syntax.Expressions;
using Nodes;
using UnityEngine;

namespace PortData {
    public record HlslVector(Expression vectorExpression) : Port.Data {
        public static VectorConstantNode DefaultNode() => new VectorConstantNode(Vector4.zero);
    }
}