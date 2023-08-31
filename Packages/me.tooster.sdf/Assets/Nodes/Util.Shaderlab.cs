#nullable enable
using System.Linq;
using AST.Shaderlab.Syntax;
using UnityEngine;

namespace Assets.Nodes {
    public static class ShaderlabUtil {
        public static ArgumentList<LiteralExpression<NumberLiteral>> VectorArgumentList(Vector4 v) =>
            new[] { v.x, v.y, v.z, v.w }.Select(
                    xi => new LiteralExpression<NumberLiteral> { literal = (FloatLiteral)xi })
                .ToArray();
    }
}
