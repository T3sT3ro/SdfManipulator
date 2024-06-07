using me.tooster.sdf.Editor.Controllers.Data;
using Unity.Properties;
using static me.tooster.sdf.AST.Hlsl.Extensions;


namespace me.tooster.sdf.Editor.Controllers.SDF.Primitives {
    [GeneratePropertyBag]
    public partial class SdfPlaneController : SdfPrimitiveController {
        public override ScalarData Apply(VectorData input, Processor processor) {
            processor.HandleRequirement(new IncludeRequirement(this, "Packages/me.tooster.sdf/Editor/Resources/Includes/primitives.hlsl"));

            return new ScalarData
            {
                evaluationExpression = FunctionCall(
                    "sdf::primitives3D::plane",
                    input.evaluationExpression
                ),
            };
        }
    }
}
