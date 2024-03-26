using System;
using System.Linq;
using me.tooster.sdf.Editor.Controllers.Data;
namespace me.tooster.sdf.Editor.Controllers.SDF.Operators {
    /// <summary>
    /// Combination operators
    /// </summary>
    public static class Combinators {
        public static SdfData CombineWithSimpleUnion(SdfData first, SdfData second) {
            return new SdfData
            {
                evaluationExpression = (vectorData) => AST.Hlsl.Extensions.FunctionCall("sdf::operators::unionSimple",
                    first.evaluationExpression(vectorData),
                    second.evaluationExpression(vectorData)),
                requiredFunctionDefinitions = first.requiredFunctionDefinitions.Concat(second.requiredFunctionDefinitions),
            };
        }

        public static SdfData CombineWithSimpleIntersect(SdfData first, SdfData second) {
            return new SdfData
            {
                evaluationExpression = vd => AST.Hlsl.Extensions.FunctionCall("sdf::operators::intersectSimple",
                    first.evaluationExpression(vd),
                    second.evaluationExpression(vd)),
                requiredFunctionDefinitions = first.requiredFunctionDefinitions.Concat(second.requiredFunctionDefinitions),
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
