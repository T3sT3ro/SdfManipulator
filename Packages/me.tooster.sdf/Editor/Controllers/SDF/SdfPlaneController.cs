using me.tooster.sdf.Editor.Controllers.Data;
using static me.tooster.sdf.AST.Hlsl.Extensions;


namespace me.tooster.sdf.Editor.Controllers.SDF {
    public class SdfPlaneController : SdfController {
        public override SdfData sdfData => new()
        {
            evaluationExpression = p => FunctionCall(sdfFunctionIdentifier, p.evaluationExpression),
            Requirements = new API.Data.Requirement[]
            {
                new HlslIncludeFileRequirement("Packages/me.tooster.sdf/Editor/Resources/Includes/primitives.hlsl"),
                createSdfFunctionRequirement(
                    p => new ScalarData
                    {
                        evaluationExpression = FunctionCall(
                            "sdf::primitives3D::plane",
                            p.evaluationExpression
                        ),
                    }
                ),
            },
        };
    }
}
