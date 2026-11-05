using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.Data;
using me.tooster.sdf.Util;
using Unity.Properties;
using UnityEngine;
using static me.tooster.sdf.AST.Hlsl.Extensions;

namespace me.tooster.sdf.Primitives {
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
            processor.HandleRequirement(new IncludeRequirement(this, SdfPaths.ShaderLibrary.Primitives));

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
