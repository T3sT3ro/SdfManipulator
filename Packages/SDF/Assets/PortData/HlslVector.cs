using API;
using Assets.Nodes;
using AST.Hlsl.Syntax.Expressions;
using UnityEngine;

namespace PortData {
    public record HlslVector(Expression vectorExpression) : API.Port.Data {
        public static VectorConstantNode DefaultNode() => DefaultNode(Vector4.zero);
        public static VectorConstantNode DefaultNode(Vector4 value) => new VectorConstantNode(value);
    }
}