using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.Editor.Controllers.Data;
using Unity.Properties;
using UnityEngine;
using static me.tooster.sdf.AST.Hlsl.Extensions;
using Type = System.Type;

namespace me.tooster.sdf.Editor.Controllers.SDF.Operators {
    /// Round SDF, simply by jumping to another isosurface
    [GeneratePropertyBag]
    public partial class SdfOnionController : Controller, IModifier<ScalarData, ScalarData> {
        static readonly                       PropertyPath thicknessProperty = new(nameof(Rounding));
        [SerializeField] [DontCreateProperty] float        thickness;

        [CreateProperty] [ShaderProperty(Description = "Thickness")]
        public float Rounding {
            get => thickness;
            set => SetField(ref thickness, value, false);
        }

        public ScalarData Apply(ScalarData input, Processor processor) {
            processor.HandleRequirement(new IncludeRequirement(this, "Packages/me.tooster.sdf/Editor/Resources/Includes/operators.hlsl"));

            return new ScalarData
            {
                evaluationExpression = FunctionCall(
                    "sdf::operators::onion_skin",
                    input.evaluationExpression,
                    (Identifier)this[thicknessProperty].identifier
                ),
            };
        }

        public override IData Apply(IData input, Processor processor) => Apply((ScalarData)input, processor);
        public override Type  GetInputType()                          => typeof(ScalarData);
        public override Type  GetOutputType()                         => typeof(ScalarData);
    }
}
