using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.Data;
using me.tooster.sdf.Util;
using Unity.Properties;
using UnityEngine;
using static me.tooster.sdf.AST.Hlsl.Extensions;

namespace me.tooster.sdf.Primitives {
    /// <summary>A regular octahedron whose vertices lie <see cref="Size" /> from its center on each axis.</summary>
    [GeneratePropertyBag]
    public partial class SdfOctahedronController : SdfPrimitiveController {
        static readonly PropertyPath sizeProperty = new(nameof(Size));

        [SerializeField] [DontCreateProperty] float size = 0.5f;

        [CreateProperty] [ShaderProperty(Description = "Octahedron size")]
        public float Size {
            get => size;
            set => SetField(ref size, value, false);
        }

        public override ScalarData Apply(VectorData input, Processor processor) {
            processor.HandleRequirement(new IncludeRequirement(this, SdfPaths.ShaderLibrary.Primitives));
            return new ScalarData
            {
                evaluationExpression = FunctionCall("sdf::primitives3D::octahedron", input.evaluationExpression, (Identifier)this[sizeProperty].identifier),
            };
        }
    }
}
