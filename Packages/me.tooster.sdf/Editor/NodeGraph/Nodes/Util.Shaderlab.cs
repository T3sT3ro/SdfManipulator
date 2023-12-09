#nullable enable
using System.Linq;
using me.tooster.sdf.AST.Shaderlab.Syntax;
using UnityEngine;

namespace me.tooster.sdf.Editor.NodeGraph.Nodes {
    public static class ShaderlabUtil {
        public static ArgumentList<LiteralExpression> VectorArgumentList(Vector4 v) =>
            new[] { v.x, v.y, v.z, v.w }.Select(
                    xi => new LiteralExpression { literal = (FloatLiteral)xi })
                .ToArray();
    }
}
