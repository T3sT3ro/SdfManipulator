using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.Data;
using me.tooster.sdf.Util;
using Unity.Properties;
using UnityEngine;
using static me.tooster.sdf.AST.Hlsl.Extensions;

namespace me.tooster.sdf.Primitives {
    /// <summary>
    ///     A box with half sizes <see cref="BoxExtents" />, whose edges and corners are rounded with the radius
    ///     <see cref="Rounding" />. The rounding keeps the size of the box.
    /// </summary>
    [GeneratePropertyBag]
    public partial class SdfBoxController : SdfPrimitiveController {
        static readonly                       PropertyPath boxExtentsProperty = new(nameof(BoxExtents));
        static readonly                       PropertyPath roundingProperty   = new(nameof(Rounding));
        [SerializeField] [DontCreateProperty] Vector3      boxExtents         = Vector3.one / 4;
        [SerializeField] [DontCreateProperty] float        rounding;

        [CreateProperty] [ShaderProperty(Description = "Box extents")]
        public Vector3 BoxExtents {
            get => boxExtents;
            set => SetField(ref boxExtents, value, false);
        }

        /// The radius of the rounded edges and corners, at most the smallest of the extents
        [CreateProperty] [ShaderProperty(Description = "Rounding")]
        public float Rounding {
            get => rounding;
            set => SetField(ref rounding, value, false);
        }

        public override ScalarData Apply(VectorData input, Processor processor) {
            processor.HandleRequirement(new IncludeRequirement(this, SdfPaths.ShaderLibrary.Primitives));

            return new ScalarData
            {
                evaluationExpression = FunctionCall(
                    "sdf::primitives3D::box_rounded",
                    input.evaluationExpression,
                    (Identifier)this[boxExtentsProperty].identifier,
                    (Identifier)this[roundingProperty].identifier
                ),
            };
        }
    }
}
