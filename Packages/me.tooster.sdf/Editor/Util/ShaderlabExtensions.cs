#nullable enable
using System.Linq;
using me.tooster.sdf.AST.Shaderlab.Syntax;
using UnityEngine;
namespace me.tooster.sdf.Editor.Util {
    public static class ShaderlabExtensions {
        /// Returns argument list like (x, y, z, w)
        public static ArgumentList<LiteralExpression> VectorArgumentList(this Vector4 v) =>
            new[] { v.x, v.y, v.z, v.w }.Select(xi => new LiteralExpression { literal = (FloatLiteral)xi })
                .ToArray();
    }
}
