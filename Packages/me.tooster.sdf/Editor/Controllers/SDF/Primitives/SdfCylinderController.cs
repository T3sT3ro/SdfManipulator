using System;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.Editor.Controllers.Data;
using Unity.Properties;
using UnityEngine;
using static me.tooster.sdf.AST.Hlsl.Extensions;

namespace me.tooster.sdf.Editor.Controllers.SDF.Primitives {
    [GeneratePropertyBag]
    public partial class SdfCylinderController : SdfPrimitiveController {
        public enum CylinderVariant {
            [InspectorName("Infinite")] INFINITE,
            [InspectorName("Capped")]   CAPPED,
            [InspectorName("Rounded")]  ROUNDED,
        }



        static readonly PropertyPath cylinderHeightProperty   = new(nameof(Height));
        static readonly PropertyPath cylinderRadiusProperty   = new(nameof(Radius));
        static readonly PropertyPath cylinderRoundingProperty = new(nameof(Rounding));

        [SerializeField] [DontCreateProperty] float           height   = 1f;
        [SerializeField] [DontCreateProperty] float           radius   = .25f;
        [SerializeField] [DontCreateProperty] float           rounding = .25f;
        [SerializeField] [DontCreateProperty] CylinderVariant variant  = CylinderVariant.CAPPED;

        [CreateProperty] [ShaderProperty(Description = "Cylinder height")]
        public float Height {
            get => height;
            set => SetField(ref height, value, false);
        }

        [CreateProperty] [ShaderProperty(Description = "Cylinder radius")]
        public float Radius {
            get => radius;
            set => SetField(ref radius, value, false);
        }

        [CreateProperty] [ShaderProperty(Description = "Rounding")]
        public float Rounding {
            get => rounding;
            set => SetField(ref rounding, value, false);
        }

        [CreateProperty] [ShaderStructural] public CylinderVariant Variant {
            get => variant;
            set => SetField(ref variant, value, true);
        }

        public override ScalarData Apply(VectorData input, Processor processor) {
            processor.HandleRequirement(new IncludeRequirement(this, "Packages/me.tooster.sdf/Editor/Resources/Includes/primitives.hlsl"));

            return Variant switch
            {
                CylinderVariant.CAPPED => new ScalarData
                {
                    evaluationExpression = FunctionCall(
                        "sdf::primitives3D::cylinder_capped",
                        input.evaluationExpression,
                        (Identifier)this[cylinderHeightProperty].identifier,
                        (Identifier)this[cylinderRadiusProperty].identifier
                    ),
                },
                CylinderVariant.INFINITE => new ScalarData
                {
                    evaluationExpression = FunctionCall(
                        "sdf::primitives3D::cylinder_infinite",
                        input.evaluationExpression,
                        (Identifier)this[cylinderRadiusProperty].identifier
                    ),
                },
                CylinderVariant.ROUNDED => new ScalarData
                {
                    evaluationExpression = FunctionCall(
                        "sdf::primitives3D::cylinder_rounded",
                        input.evaluationExpression,
                        (Identifier)this[cylinderRadiusProperty].identifier,
                        (Identifier)this[cylinderRoundingProperty].identifier,
                        (Identifier)this[cylinderHeightProperty].identifier
                    ),
                },
                _ => throw new ArgumentOutOfRangeException(),
            };
        }
    }
}
