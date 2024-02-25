#nullable enable
using System.Linq;
using me.tooster.sdf.AST.Shaderlab.Syntax;
using UnityEngine;
namespace me.tooster.sdf.Editor.Util {
    public static class ShaderlabExtensions {
        /// Returns argument list like (x, y, z, w)
        public static ArgumentList<LiteralExpression> VectorArgumentList(this Vector4 v) => VectorArgumentList(v.x, v.y, v.z, v.w);

        public static ArgumentList<LiteralExpression> VectorArgumentList(this Vector3 v) => VectorArgumentList(v.x, v.y, v.z);

        public static ArgumentList<LiteralExpression> VectorArgumentList(this Vector2 v) => VectorArgumentList(v.x, v.y);

        public static ArgumentList<LiteralExpression> VectorArgumentList(this Vector3Int v) => VectorArgumentList(v.x, v.y, v.z);

        public static ArgumentList<LiteralExpression> VectorArgumentList(this Vector2Int v) => VectorArgumentList(v.x, v.y);

        public static ArgumentList<LiteralExpression> VectorArgumentList(float x, float y = 0, float z = 0, float w = 0) =>
            new[] { x, y, z, w }.Select(xi => new LiteralExpression { literal = (FloatLiteral)xi })
                .ToArray();
    }
}
