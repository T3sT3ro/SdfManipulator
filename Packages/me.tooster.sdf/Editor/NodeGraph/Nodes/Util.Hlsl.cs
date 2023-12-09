#nullable enable

using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST;
using me.tooster.sdf.AST.Hlsl;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions;
using me.tooster.sdf.AST.Syntax;
using UnityEngine;

namespace me.tooster.sdf.Editor.NodeGraph.Nodes {
    public static class HlslUtil {
        // convert typed enumerables to literal enumerables
        private static IEnumerable<LiteralExpression> ToLiteralList(this IEnumerable<float> vals) =>
            vals.Select(v => new LiteralExpression { literal = (FloatLiteral)v }).AsEnumerable();

        private static IEnumerable<LiteralExpression> ToLiteralList(this IEnumerable<int> vals) =>
            vals.Select(v => new LiteralExpression { literal = (IntLiteral)v }).AsEnumerable();

        private static IEnumerable<LiteralExpression> ToLiteralList(this IEnumerable<bool> vals) =>
            vals.Select(v => new LiteralExpression { literal = (BooleanLiteral)v }).AsEnumerable();
        
        // (x, y, z, w)
        public static AST.Hlsl.Syntax.ArgumentList<Syntax<hlsl>> VectorArgumentList(Vector4 vec) =>
            new[] { vec.x, vec.y, vec.z, vec.w }.ToLiteralList().Cast<Syntax<hlsl>>().ToArgumentList();

        // {a, b, c}
        public static AST.Hlsl.Syntax.BracedList<Syntax<hlsl>> VectorInitializerList(Vector4 vec) =>
            new[] { vec.x, vec.y, vec.z, vec.w }.ToLiteralList().Cast<Syntax<hlsl>>().ToBracedList();
        
        // {{a,b,c,d}, {...}, {...}, {...}}
        public static AST.Hlsl.Syntax.BracedList<AST.Hlsl.Syntax.BracedList<Syntax<hlsl>>>
            MatrixInitializerList(Matrix4x4 value) =>
            new[] { value.GetRow(0), value.GetRow(1), value.GetRow(2), value.GetRow(3) }
                .Select(VectorInitializerList)
                .ToBracedList();
    }
}
