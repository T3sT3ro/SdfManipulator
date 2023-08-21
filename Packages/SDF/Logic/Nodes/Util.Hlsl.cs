#nullable enable

using System.Collections.Generic;
using System.Linq;
using AST.Hlsl;
using AST.Hlsl.Syntax;
using AST.Hlsl.Syntax.Expressions;
using UnityEngine;

namespace Nodes {
    public static class HlslUtil {
        // convert typed enumerables to literal enumerables
        private static IEnumerable<LiteralExpression<Literal>> ToLiteralList(this IEnumerable<float> vals) =>
            vals.Select(v => new LiteralExpression<Literal> { literal = (FloatLiteral)v }).AsEnumerable();

        private static IEnumerable<LiteralExpression<Literal>> ToLiteralList(this IEnumerable<int> vals) =>
            vals.Select(v => new LiteralExpression<Literal> { literal = (IntLiteral)v }).AsEnumerable();

        private static IEnumerable<LiteralExpression<Literal>> ToLiteralList(this IEnumerable<bool> vals) =>
            vals.Select(v => new LiteralExpression<Literal> { literal = (BooleanLiteral)v }).AsEnumerable();

        
        // convert enumerables to appropriate lists
        public static ArgumentList<T> ToArgumentList<T>(this IEnumerable<T> args)
            where T : HlslSyntax => ArgumentList<T>.Of(args);

        public static BracketInitializerList<T> ToBracketInitializerList<T>(this IEnumerable<T> args)
            where T : HlslSyntax => BracketInitializerList<T>.Of(args);
        
        
        // (x, y, z, w)
        public static ArgumentList<HlslSyntax> VectorArgumentList(Vector4 vec) =>
            new[] { vec.x, vec.y, vec.z, vec.w }.ToLiteralList().ToArray();

        // {a, b, c}
        public static BracketInitializerList<HlslSyntax> VectorInitializerList(Vector4 vec) =>
            new[] { vec.x, vec.y, vec.z, vec.w }.ToLiteralList().ToArray();
        
        // {{a,b,c,d}, {...}, {...}, {...}}
        public static BracketInitializerList<BracketInitializerList<HlslSyntax>>
            MatrixInitializerList(Matrix4x4 value) =>
            new[] { value.GetRow(0), value.GetRow(1), value.GetRow(2), value.GetRow(3) }
                .Select(VectorInitializerList)
                .ToBracketInitializerList();
    }
}
