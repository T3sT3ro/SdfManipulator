using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.Editor.Controllers.Data;
using Unity.Properties;
using UnityEngine;
using static me.tooster.sdf.AST.Hlsl.Extensions;

namespace me.tooster.sdf.Editor.Controllers.SDF.Primitives {
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
            processor.HandleRequirement(new IncludeRequirement(this, "Packages/me.tooster.sdf/Editor/Resources/Includes/primitives.hlsl"));

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
