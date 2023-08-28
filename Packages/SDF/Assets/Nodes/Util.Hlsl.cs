#nullable enable

using System.Collections.Generic;
using System.Linq;
using AST.Hlsl;
using AST.Hlsl.Syntax;
using AST.Hlsl.Syntax.Expressions;
using AST.Syntax;
using UnityEngine;

namespace Assets.Nodes {
    public static class HlslUtil {
        // convert typed enumerables to literal enumerables
        private static IEnumerable<LiteralExpression<Literal>> ToLiteralList(this IEnumerable<float> vals) =>
            vals.Select(v => new LiteralExpression<Literal> { literal = (FloatLiteral)v }).AsEnumerable();

        private static IEnumerable<LiteralExpression<Literal>> ToLiteralList(this IEnumerable<int> vals) =>
            vals.Select(v => new LiteralExpression<Literal> { literal = (IntLiteral)v }).AsEnumerable();

        private static IEnumerable<LiteralExpression<Literal>> ToLiteralList(this IEnumerable<bool> vals) =>
            vals.Select(v => new LiteralExpression<Literal> { literal = (BooleanLiteral)v }).AsEnumerable();
        
        // (x, y, z, w)
        public static ArgumentList<Syntax<Hlsl>> VectorArgumentList(Vector4 vec) =>
            new[] { vec.x, vec.y, vec.z, vec.w }.ToLiteralList().Cast<Syntax<Hlsl>>().ToArgumentList();

        // {a, b, c}
        public static BracedList<Syntax<Hlsl>> VectorInitializerList(Vector4 vec) =>
            new[] { vec.x, vec.y, vec.z, vec.w }.ToLiteralList().Cast<Syntax<Hlsl>>().ToBracedList();
        
        // {{a,b,c,d}, {...}, {...}, {...}}
        public static BracedList<BracedList<Syntax<Hlsl>>>
            MatrixInitializerList(Matrix4x4 value) =>
            new[] { value.GetRow(0), value.GetRow(1), value.GetRow(2), value.GetRow(3) }
                .Select(VectorInitializerList)
                .ToBracedList();
    }
}
