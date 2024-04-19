using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST;
using me.tooster.sdf.AST.Hlsl;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions;
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators;
using me.tooster.sdf.AST.Syntax.CommonSyntax;
using UnityEngine;
namespace me.tooster.sdf.Editor.Util {
    public static class HlslExtensions {
        // convert typed enumerables to literal enumerables
        static IEnumerable<LiteralExpression> ToLiterals(this IEnumerable<float> vals)
            => vals.Select(v => new LiteralExpression { literal = (FloatLiteral)v }).AsEnumerable();

        static IEnumerable<LiteralExpression> ToLiterals(this IEnumerable<int> vals)
            => vals.Select(v => new LiteralExpression { literal = (IntLiteral)v }).AsEnumerable();

        static IEnumerable<LiteralExpression> ToLiterals(this IEnumerable<bool> vals)
            => vals.Select(v => new LiteralExpression { literal = (BooleanLiteral)v }).AsEnumerable();

        public static Call VectorConstructor(params int[] xs)
            => new() { calee = $"{Constants.ScalarKind.@int}{xs.Length}", argList = xs.Select(x => (LiteralExpression)x).ToArray() };

        public static Call VectorConstructor(params float[] xs)
            => new() { calee = $"{Constants.ScalarKind.@float}{xs.Length}", argList = xs.Select(x => (LiteralExpression)x).ToArray() };

        public static Call VectorConstructor(params bool[] xs)
            => new() { calee = $"{Constants.ScalarKind.@bool}{xs.Length}", argList = xs.Select(x => (LiteralExpression)x).ToArray() };

        public static Call VectorConstructor(Vector2 v)    => VectorConstructor(v.x, v.y);
        public static Call VectorConstructor(Vector3 v)    => VectorConstructor(v.x, v.y, v.z);
        public static Call VectorConstructor(Vector4 v)    => VectorConstructor(v.x, v.y, v.z, v.w);
        public static Call VectorConstructor(Vector2Int v) => VectorConstructor(v.x, v.y);
        public static Call VectorConstructor(Vector3Int v) => VectorConstructor(v.x, v.y, v.z);

        /// constructs braced initializer expression for vector: {x, y, z, w}
        public static BracedInitializerExpression ToBracedInitializerList(this Vector4 vec)
            => new[] { vec.x, vec.y, vec.z, vec.w }.ToLiterals().Cast<Expression<hlsl>>().ToBracedList();

        /// constructs braced innitializer syntax for this matrix looking like: <code>{{m11,m12,m13,m14}, {...}, {...}, {...}}</code>
        public static BracedInitializerExpression ToMatrixInitializerList(this Matrix4x4 value)
            => Enumerable.Range(0, 4).Select(i => value.GetRow(i).ToBracedInitializerList() as Expression<hlsl>).ToBracedList();
    }
}
