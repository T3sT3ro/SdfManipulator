using System;
using System.Linq;
using me.tooster.sdf.AST;
using me.tooster.sdf.AST.Syntax.CommonSyntax;
namespace me.tooster.sdf.Editor.Controllers.Data {
    /// <summary>
    /// Combination operators
    /// </summary>
    public static class Combinators {
        public static SdfData CombineWithExternalFunction(string function, SdfData first, SdfData second, params Expression<hlsl>[] rest) {
            return new SdfData
            {
                evaluationExpression = (vectorData) => AST.Hlsl.Extensions.FunctionCall(
                    function,
                    new[]
                    {
                        first.evaluationExpression(vectorData),
                        second.evaluationExpression(vectorData),
                    }.Concat(rest).ToArray()
                ),
                Requirements = first.Requirements.Concat(second.Requirements),
            };
        }

        /// generates an sdf data that is a simple union of all sdf datas given using the supplied combinator function
        public static SdfData binaryCombine(Func<SdfData, SdfData, SdfData> combinator, ArraySegment<SdfData> sdfDatas) {
            switch (sdfDatas.Count) {
                case 0: throw new ArgumentException("At least one sdf data is required to combine");
                case 1: return sdfDatas.get_Item(0);
                default: {
                    var mid = sdfDatas.Count / 2;
                    return combinator(binaryCombine(combinator, sdfDatas[..mid]), binaryCombine(combinator, sdfDatas[mid..]));
                }
            }
        }
    }
}
