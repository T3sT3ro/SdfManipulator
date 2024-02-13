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
        private static IEnumerable<LiteralExpression> ToLiterals(this IEnumerable<float> vals) =>
            vals.Select(v => new LiteralExpression { literal = (FloatLiteral)v }).AsEnumerable();
        private static IEnumerable<LiteralExpression> ToLiterals(this IEnumerable<int> vals) =>
            vals.Select(v => new LiteralExpression { literal = (IntLiteral)v }).AsEnumerable();
        private static IEnumerable<LiteralExpression> ToLiterals(this IEnumerable<bool> vals) =>
            vals.Select(v => new LiteralExpression { literal = (BooleanLiteral)v }).AsEnumerable();


        /// constructs argument list like so: (x, y, z, w)
        public static ArgumentList<Expression<hlsl>> ToVectorConstructor(
            this Vector4 vec,
            Constants.ScalarKind kind = Constants.ScalarKind.@float
        ) => new Call
        {
            calee = $"{kind}4",
            argList = new[] { vec.x, vec.y, vec.z, vec.w }.ToLiterals().Cast<Expression<hlsl>>().ToArgumentList(),
        };

        /// constructs braced initializer expression for vector: {x, y, z, w}
        public static BracedInitializerExpression ToBracedInitializerList(this Vector4 vec) =>
            new[] { vec.x, vec.y, vec.z, vec.w }.ToLiterals().Cast<Expression<hlsl>>().ToBracedList();

        /// constructs braced innitializer syntax for this matrix looking like: <code>{{m11,m12,m13,m14}, {...}, {...}, {...}}</code>
        public static BracedInitializerExpression ToMatrixInitializerList(this Matrix4x4 value) =>
            Enumerable.Range(0, 4).Select(i => value.GetRow(i).ToBracedInitializerList() as Expression<hlsl>).ToBracedList();
    }
}
