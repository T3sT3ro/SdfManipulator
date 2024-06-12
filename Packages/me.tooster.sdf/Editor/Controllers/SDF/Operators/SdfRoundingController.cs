using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.Editor.Controllers.Data;
using Unity.Properties;
using UnityEngine;
using static me.tooster.sdf.AST.Hlsl.Extensions;
using Type = System.Type;

namespace me.tooster.sdf.Editor.Controllers.SDF.Operators {
    /// Round SDF, simply by jumping to another isosurface
    [GeneratePropertyBag]
    public partial class SdfRoundingController : Controller, IModifier<ScalarData, ScalarData> {
        static readonly                       PropertyPath roundingProperty = new(nameof(Rounding));
        [SerializeField] [DontCreateProperty] float        rounding;

        [CreateProperty] [ShaderProperty(Description = "Rounding")]
        public float Rounding {
            get => rounding;
            set => SetField(ref rounding, value, false);
        }

        public ScalarData Apply(ScalarData input, Processor processor) {
            processor.HandleRequirement(new IncludeRequirement(this, "Packages/me.tooster.sdf/Editor/Resources/Includes/operators.hlsl"));

            return new ScalarData
            {
                evaluationExpression = FunctionCall(
                    "sdf::operators::round_sdf",
                    input.evaluationExpression,
                    (Identifier)this[roundingProperty].identifier
                ),
            };
        }

        public override IData Apply(IData input, Processor processor) => Apply((ScalarData)input, processor);
        public override Type  GetInputType()                          => typeof(ScalarData);
        public override Type  GetOutputType()                         => typeof(ScalarData);
    }
}
