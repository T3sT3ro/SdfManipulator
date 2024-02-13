using System;
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators;
using me.tooster.sdf.Editor.Controllers.Data;

namespace me.tooster.sdf.Editor.Controllers {
    /// <summary>
    /// Combination operators
    /// </summary>
    public static class Combinators {
        private static SdfData CombineWithSimpleUnion(SdfData first, SdfData second) {
            return new SdfData
            {
                EvaluationExpression = (vectorData) => new Call
                {
                    calee = "sdf::uperators::union",
                    argList = new[]
                    {
                        first.EvaluationExpression(vectorData),
                        second.EvaluationExpression(vectorData),
                    },
                },
            };
        }

        /// generates an sdf data that is a simple union of all sdf datas given using the min operator
        public static SdfData binaryCombine(Func<SdfData, SdfData, SdfData> combinator, ArraySegment<SdfData> sdfDatas) {
            // aggregate with `SdfResult sdf::uperators::union(SdfResult a, SdfResult b)` using a linear application
            if (sdfDatas.Count == 0) throw new ArgumentException("At least one sdf data is required");
            var mid = sdfDatas.Count / 2;
            return combinator(binaryCombine(combinator, sdfDatas[..mid]), binaryCombine(combinator, sdfDatas[mid..]));
        }
    }
}
