using System;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.Editor.Controllers.Data;
using Unity.Properties;
using UnityEngine;
using static me.tooster.sdf.AST.Hlsl.Extensions;

namespace me.tooster.sdf.Editor.Controllers.SDF {
    [GeneratePropertyBag]
    public partial class SdfTorusController : SdfController {
        public enum TorusVariant {
            [InspectorName("Normal")] NORMAL,
            [InspectorName("Capped")] CAPPED,
        }



        static readonly PropertyPath torusMainRadius = new(nameof(MainRadius));
        static readonly PropertyPath torusRingRadius = new(nameof(RingRadius));
        static readonly PropertyPath torusCap        = new(nameof(Cap));

        [SerializeField] [DontCreateProperty] float        mainRadius = 1f;
        [SerializeField] [DontCreateProperty] float        ringRadius = 0.25f;
        [SerializeField] [DontCreateProperty] Vector2      cap        = new(0.1f, 0.1f);
        [SerializeField] [DontCreateProperty] TorusVariant variant    = TorusVariant.NORMAL;


        [CreateProperty] [ShaderProperty(Description = "Torus main radius")]
        public float MainRadius {
            get => mainRadius;
            set => SetField(ref mainRadius, value, false);
        }

        [CreateProperty] [ShaderProperty(Description = "Torus ring radius")]
        public float RingRadius {
            get => ringRadius;
            set => SetField(ref ringRadius, value, false);
        }

        [CreateProperty] [ShaderProperty(Description = "Torus cap")]
        public Vector2 Cap {
            get => cap;
            set => SetField(ref cap, value, false);
        }

        [CreateProperty] [ShaderStructural] public TorusVariant Variant {
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
                    TorusVariant.NORMAL =>
                        createSdfFunctionRequirement(
                            p => new ScalarData
                            {
                                evaluationExpression = FunctionCall(
                                    "sdf::primitives3D::torus",
                                    p.evaluationExpression,
                                    new Identifier { id = this[torusMainRadius].identifier },
                                    new Identifier { id = this[torusRingRadius].identifier }
                                ),
                            }
                        ),
                    TorusVariant.CAPPED =>
                        createSdfFunctionRequirement(
                            p => new ScalarData
                            {
                                evaluationExpression = FunctionCall(
                                    "sdf::primitives3D::torus_capped",
                                    p.evaluationExpression,
                                    new Identifier { id = this[torusMainRadius].identifier },
                                    new Identifier { id = this[torusRingRadius].identifier },
                                    new Identifier { id = this[torusCap].identifier }
                                ),
                            }
                        ),
                    _ => throw new ArgumentOutOfRangeException(),
                },
            },
        };
    }
}
