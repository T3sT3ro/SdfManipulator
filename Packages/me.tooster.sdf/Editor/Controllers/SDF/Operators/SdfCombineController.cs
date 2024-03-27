using System;
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.Editor.API;
using me.tooster.sdf.Editor.Controllers.Data;
using me.tooster.sdf.Editor.Util;
using UnityEditor;
namespace me.tooster.sdf.Editor.Controllers.SDF.Operators {
    public class SdfCombineController : SdfController {
        public enum Mode {
            SIMPLE_UNION, INTERSECTION, SMOOTH_UNION,
        }

        public Mode            mode        = Mode.SIMPLE_UNION;
        public Property<float> blendFactor = new("blendFactor", "Blend factor", 1);

        public override    IEnumerable<Property> Properties   => base.Properties.Append(blendFactor);
        protected override void                  OnValidate() { UpdateBlendFactor(); }

        private void UpdateBlendFactor() {
            blendFactor.Value = transform.localScale.x;
            SdfScene.QueuePropertyUpdates(blendFactor);
        }

        public override SdfData sdfData => Combinators.binaryCombine(
            MergeData,
            gameObject.GetImmediateChildrenComponents<SdfController>()
                .Where(c => c != this)
                .Select(p => p.sdfData).ToArray()
        );

        private SdfData MergeData(SdfData d1, SdfData d2) => mode switch
        {
            Mode.SIMPLE_UNION => Combinators.CombineWithSimpleUnion(d1, d2),
            Mode.INTERSECTION => Combinators.CombineWithSimpleIntersect(d1, d2),
            Mode.SMOOTH_UNION => new SdfData
            {
                evaluationExpression = vd => AST.Hlsl.Extensions.FunctionCall("sdf::operators::unionSmooth",
                    d1.evaluationExpression(vd),
                    d2.evaluationExpression(vd),
                    (Identifier)SdfScene.GetPropertyIdentifier(blendFactor)
                ),
                requiredFunctionDefinitions = d1.requiredFunctionDefinitions.Concat(d2.requiredFunctionDefinitions),
            },
            _ => throw new ArgumentOutOfRangeException(),
        };

        [MenuItem("GameObject/SDF/Operators/Combine", priority = 200)]
        public static void CreateSdfBox() => TryCreateController<SdfCombineController>("combine");
    }
}
