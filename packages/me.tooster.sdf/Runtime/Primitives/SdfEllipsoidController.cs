using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.Data;
using me.tooster.sdf.Util;
using Unity.Properties;
using UnityEngine;
using static me.tooster.sdf.AST.Hlsl.Extensions;

namespace me.tooster.sdf.Primitives {
    /// <summary>An ellipsoid with a radius for each axis.</summary>
    [GeneratePropertyBag]
    public partial class SdfEllipsoidController : SdfPrimitiveController {
        static readonly PropertyPath radiiProperty = new(nameof(Radii));

        [SerializeField] [DontCreateProperty] Vector3 radii = new(0.5f, 0.3f, 0.3f);

        [CreateProperty] [ShaderProperty(Description = "Ellipsoid radii")]
        public Vector3 Radii {
            get => radii;
            set => SetField(ref radii, value, false);
        }

        public override ScalarData Apply(VectorData input, Processor processor) {
            processor.HandleRequirement(new IncludeRequirement(this, SdfPaths.ShaderLibrary.Primitives));
            return new ScalarData
            {
                evaluationExpression = FunctionCall("sdf::primitives3D::ellipsoid", input.evaluationExpression, (Identifier)this[radiiProperty].identifier),
            };
        }
    }
}
