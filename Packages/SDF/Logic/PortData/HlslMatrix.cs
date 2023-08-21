using API;
using AST.Hlsl.Syntax.Expressions;
using Nodes;
using UnityEngine;

namespace PortData {
    public record HlslMatrix(Expression matrixExpression) : Port.Data {
        public static MatrixConstantNode DefaultNode() => new MatrixConstantNode(Matrix4x4.identity);
    }
}