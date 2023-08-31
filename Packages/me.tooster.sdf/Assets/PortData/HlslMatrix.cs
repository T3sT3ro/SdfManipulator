using Assets.Nodes;
using AST.Hlsl;
using UnityEngine;

namespace PortData {
    public record HlslMatrix(Expression matrixExpression) : API.Port.Data {
        public static MatrixConstantNode DefaultNode()                => DefaultNode(Matrix4x4.identity);
        public static MatrixConstantNode DefaultNode(Matrix4x4 value) => new MatrixConstantNode(value);
    }
}