using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.Data;
using me.tooster.sdf.Util;
using Unity.Properties;
using UnityEngine;
using static me.tooster.sdf.AST.Hlsl.Extensions;

namespace me.tooster.sdf.Primitives {
    /// <summary>A capsule along the y axis: a segment from -<see cref="Height" /> to <see cref="Height" />, rounded with <see cref="Radius" />.</summary>
    [GeneratePropertyBag]
    public partial class SdfCapsuleController : SdfPrimitiveController {
        static readonly PropertyPath heightProperty = new(nameof(Height));
        static readonly PropertyPath radiusProperty = new(nameof(Radius));

        [SerializeField] [DontCreateProperty] float height = 0.5f;
        [SerializeField] [DontCreateProperty] float radius = 0.25f;

        /// Half the length of the straight part
        [CreateProperty] [ShaderProperty(Description = "Capsule height")]
        public float Height {
            get => height;
            set => SetField(ref height, value, false);
        }

        [CreateProperty] [ShaderProperty(Description = "Capsule radius")]
        public float Radius {
            get => radius;
            set => SetField(ref radius, value, false);
        }

        public override ScalarData Apply(VectorData input, Processor processor) {
            processor.HandleRequirement(new IncludeRequirement(this, SdfPaths.ShaderLibrary.Primitives));
            return new ScalarData
            {
                evaluationExpression = FunctionCall(
                    "sdf::primitives3D::capsule",
                    input.evaluationExpression,
                    (Identifier)this[heightProperty].identifier,
                    (Identifier)this[radiusProperty].identifier
                ),
            };
        }
    }
}
