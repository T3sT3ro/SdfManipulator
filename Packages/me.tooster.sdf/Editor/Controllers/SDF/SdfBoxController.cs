using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.Editor.Controllers.Data;
using Unity.Properties;
using UnityEngine;
using static me.tooster.sdf.AST.Hlsl.Extensions;

namespace me.tooster.sdf.Editor.Controllers.SDF {
    public partial class SdfBoxController : SdfController {
        static readonly                       PropertyPath boxExtentsPropertyPath = new(nameof(BoxExtents));
        [SerializeField] [DontCreateProperty] Vector3      boxExtents             = Vector3.one / 4;

        [CreateProperty] [ShaderProperty(Description = "Box extents")]
        public Vector3 BoxExtents {
            get => boxExtents;
            set => SetField(ref boxExtents, value, false);
        }

        public override SdfData sdfData => new()
        {
            evaluationExpression = p => FunctionCall(sdfFunctionIdentifier, p.evaluationExpression),
            Requirements = new API.Data.Requirement[]
            {
                new HlslIncludeFileRequirement("Packages/me.tooster.sdf/Editor/Resources/Includes/primitives.hlsl"),
                createSdfFunctionRequirement(
                    pd => new ScalarData
                    {
                        evaluationExpression = FunctionCall(
                            "sdf::primitives3D::box",
                            pd.evaluationExpression,
                            new Identifier { id = SdfScene.sceneData.controllers[this].properties[boxExtentsPropertyPath].identifier }
                        ),
                    }
                ),
            },
        };
    }
}
