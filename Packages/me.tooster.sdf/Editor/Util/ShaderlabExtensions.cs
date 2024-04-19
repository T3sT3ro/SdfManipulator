#nullable enable
using me.tooster.sdf.AST.Shaderlab.Syntax;
using UnityEngine;
namespace me.tooster.sdf.Editor.Util {
    public static class ShaderlabExtensions {
        /// Returns approperiate argument list with float literals for given arguments, e.g. (7.0, 8.0, 9.0, 10.5) or (0, 1, 0)
        public static ArgumentList<LiteralExpression> VectorArgumentList(this Vector4 v) => VectorArgumentList(v.x, v.y, v.z, v.w);

        /// <inheritdoc cref="VectorArgumentList(UnityEngine.Vector4)"/>
        public static ArgumentList<LiteralExpression> VectorArgumentList(this Vector3 v) => VectorArgumentList(v.x, v.y, v.z);

        /// <inheritdoc cref="VectorArgumentList(UnityEngine.Vector4)"/>
        public static ArgumentList<LiteralExpression> VectorArgumentList(this Vector2 v) => VectorArgumentList(v.x, v.y);

        /// <inheritdoc cref="VectorArgumentList(UnityEngine.Vector4)"/>
        public static ArgumentList<LiteralExpression> VectorArgumentList(this Vector3Int v) => VectorArgumentList(v.x, v.y, v.z);

        /// <inheritdoc cref="VectorArgumentList(UnityEngine.Vector4)"/>
        public static ArgumentList<LiteralExpression> VectorArgumentList(this Vector2Int v) => VectorArgumentList(v.x, v.y);

        /// <inheritdoc cref="VectorArgumentList(UnityEngine.Vector4)"/>
        public static ArgumentList<LiteralExpression> VectorArgumentList(float x, float y = 0, float z = 0, float w = 0) => new(x, y, z, w);
    }
}
