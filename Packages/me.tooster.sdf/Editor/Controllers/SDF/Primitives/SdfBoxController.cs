using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.Editor.Controllers.Data;
using Unity.Properties;
using UnityEngine;
using static me.tooster.sdf.AST.Hlsl.Extensions;

namespace me.tooster.sdf.Editor.Controllers.SDF.Primitives {
    [GeneratePropertyBag]
    public partial class SdfBoxController : SdfPrimitiveController {
        static readonly                       PropertyPath boxExtentsProperty = new(nameof(BoxExtents));
        [SerializeField] [DontCreateProperty] Vector3      boxExtents         = Vector3.one / 4;

        [CreateProperty] [ShaderProperty(Description = "Box extents")]
        public Vector3 BoxExtents {
            get => boxExtents;
            set => SetField(ref boxExtents, value, false);
        }

        public override ScalarData Apply(VectorData input, Processor processor) {
            processor.HandleRequirement(new IncludeRequirement(this, "Packages/me.tooster.sdf/Editor/Resources/Includes/primitives.hlsl"));

            return new ScalarData
            {
                evaluationExpression = FunctionCall(
                    "sdf::primitives3D::box",
                    input.evaluationExpression,
                    (Identifier)this[boxExtentsProperty].identifier
                ),
            };
        }
    }
}
