#nullable enable
using System.Linq;
using AST.Shaderlab.Syntax;
using UnityEngine;

namespace Nodes {
    public static partial class Util {
        public static class Shaderlab {
            public static ArgumentList<LiteralExpression<NumberLiteral>> VectorArgumentList(Vector4 v) =>
                new[] { v.x, v.y, v.z, v.w }.Select(
                    xi => new LiteralExpression<NumberLiteral> { literal = (FloatLiteral)xi })
                    .ToArray();
        }
    }
}
