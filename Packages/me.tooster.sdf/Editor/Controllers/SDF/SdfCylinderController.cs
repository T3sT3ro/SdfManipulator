using System;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.Editor.Controllers.Data;
using Unity.Properties;
using UnityEngine;
using static me.tooster.sdf.AST.Hlsl.Extensions;

namespace me.tooster.sdf.Editor.Controllers.SDF {
    [GeneratePropertyBag]
    public partial class SdfCylinderController : SdfController {
        public enum CylinderVariant {
            [InspectorName("Infinite")] INFINITE,
            [InspectorName("Capped")]   CAPPED,
            [InspectorName("Rounded")]  ROUNDED,
        }



        static readonly PropertyPath cylinderHeight   = new(nameof(Height));
        static readonly PropertyPath cylinderRadius   = new(nameof(Radius));
        static readonly PropertyPath cylinderRounding = new(nameof(Rounding));

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

        public override SdfData sdfData => new()
        {
            evaluationExpression = p => FunctionCall(controllerIdentifier, p.evaluationExpression),
            Requirements = new API.Data.Requirement[]
            {
                new HlslIncludeFileRequirement("Packages/me.tooster.sdf/Editor/Resources/Includes/primitives.hlsl"),
                Variant switch
                {
                    CylinderVariant.CAPPED => createSdfFunctionRequirement(
                        p => new ScalarData
                        {
                            evaluationExpression = FunctionCall(
                                "sdf::primitives3D::cylinder_capped",
                                p.evaluationExpression,
                                (Identifier)this[cylinderHeight].identifier,
                                (Identifier)this[cylinderRadius].identifier
                            ),
                        }
                    ),
                    CylinderVariant.INFINITE => createSdfFunctionRequirement(
                        p => new ScalarData
                        {
                            evaluationExpression = FunctionCall(
                                "sdf::primitives3D::cylinder_infinite",
                                p.evaluationExpression,
                                (Identifier)this[cylinderRadius].identifier
                            ),
                        }
                    ),
                    CylinderVariant.ROUNDED => createSdfFunctionRequirement(
                        p => new ScalarData
                        {
                            evaluationExpression = FunctionCall(
                                "sdf::primitives3D::cylinder_rounded",
                                p.evaluationExpression,
                                (Identifier)this[cylinderRadius].identifier,
                                (Identifier)this[cylinderRounding].identifier,
                                (Identifier)this[cylinderHeight].identifier
                            ),
                        }
                    ),
                    _ => throw new ArgumentOutOfRangeException(),
                },
            },
        };
    }
}
