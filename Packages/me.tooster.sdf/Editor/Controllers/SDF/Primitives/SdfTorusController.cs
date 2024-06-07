using System;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.Editor.Controllers.Data;
using Unity.Properties;
using UnityEngine;
using static me.tooster.sdf.AST.Hlsl.Extensions;

namespace me.tooster.sdf.Editor.Controllers.SDF.Primitives {
    [GeneratePropertyBag]
    public partial class SdfTorusController : SdfPrimitiveController {
        public enum TorusVariant {
            [InspectorName("Normal")] NORMAL,
            [InspectorName("Capped")] CAPPED,
        }



        static readonly PropertyPath torusMainRadiusProperty = new(nameof(MainRadius));
        static readonly PropertyPath torusRingRadiusProperty = new(nameof(RingRadius));
        static readonly PropertyPath torusCapProperty        = new(nameof(Cap));

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

        public override ScalarData Apply(VectorData input, Processor processor) {
            processor.HandleRequirement(new IncludeRequirement(this, "Packages/me.tooster.sdf/Editor/Resources/Includes/primitives.hlsl"));

            var mainRadiusUniform = (Identifier)this[torusMainRadiusProperty].identifier;
            var ringRadiusUniform = (Identifier)this[torusRingRadiusProperty].identifier;

            return Variant switch
            {
                TorusVariant.NORMAL => new ScalarData
                {
                    evaluationExpression = FunctionCall(
                        "sdf::primitives3D::torus",
                        input.evaluationExpression,
                        mainRadiusUniform,
                        ringRadiusUniform
                    ),
                },
                TorusVariant.CAPPED => new ScalarData
                {
                    evaluationExpression = FunctionCall(
                        "sdf::primitives3D::torus_capped",
                        input.evaluationExpression,
                        mainRadiusUniform,
                        ringRadiusUniform,
                        (Identifier)this[torusCapProperty].identifier
                    ),
                },
                _ => throw new ArgumentOutOfRangeException(),
            };
        }
    }
}
