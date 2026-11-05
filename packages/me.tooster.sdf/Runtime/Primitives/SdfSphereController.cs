using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.Data;
using me.tooster.sdf.Util;
using Unity.Properties;
using UnityEngine;
using static me.tooster.sdf.AST.Hlsl.Extensions;

namespace me.tooster.sdf.Primitives {
    [GeneratePropertyBag]
    public partial class SdfSphereController : SdfPrimitiveController {
        static readonly                       PropertyPath radiusProperty = new(nameof(Radius));
        [SerializeField] [DontCreateProperty] float        radius         = 1f;

        [CreateProperty] [ShaderProperty(Description = "Sphere radius")]
        public float Radius {
            get => radius;
            set => SetField(ref radius, value, false);
        }

        public override ScalarData Apply(VectorData input, Processor processor) {
            processor.HandleRequirement(new IncludeRequirement(this, SdfPaths.ShaderLibrary.Primitives));

            return new ScalarData
            {
                evaluationExpression = FunctionCall(
                    "sdf::primitives3D::sphere",
                    input.evaluationExpression,
                    (Identifier)this[radiusProperty].identifier
                ),
            };
        }
    }
}
