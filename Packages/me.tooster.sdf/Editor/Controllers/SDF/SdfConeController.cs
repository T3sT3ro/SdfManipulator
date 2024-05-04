using System;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.Editor.Controllers.Data;
using Unity.Properties;
using UnityEngine;
using static me.tooster.sdf.AST.Hlsl.Extensions;

namespace me.tooster.sdf.Editor.Controllers.SDF {
    [GeneratePropertyBag]
    public partial class SdfConeController : SdfController {
        public enum OriginPosition {
            [InspectorName("Origin at base")]    BOTTOM,
            [InspectorName("Origin at the tip")] TIP,
        }



        static readonly PropertyPath coneAngle  = new(nameof(Angle));
        static readonly PropertyPath coneHeight = new(nameof(Height));

        [SerializeField] [DontCreateProperty] float          angle  = (float)(Math.PI / 4);
        [SerializeField] [DontCreateProperty] float          height = 1f;
        [SerializeField] [DontCreateProperty] OriginPosition coneOrigin;

        [CreateProperty] [ShaderProperty(Description = "Cone angle")]
        public float Angle {
            get => angle;
            set => SetField(ref angle, value, false);
        }

        [CreateProperty] [ShaderProperty(Description = "Cone height")]
        public float Height {
            get => height;
            set => SetField(ref height, value, false);
        }

        [CreateProperty] [ShaderStructural] public OriginPosition ConeOrigin {
            get => coneOrigin;
            set => SetField(ref coneOrigin, value, false);
        }

        [CreateProperty] [ShaderProperty]
        public override Matrix4x4 SpaceTransform => ConeOrigin switch
        {
            OriginPosition.BOTTOM => (Matrix4x4.Translate(transform.up * Height) * transform.localToWorldMatrix).inverse,
            OriginPosition.TIP    => transform.localToWorldMatrix.inverse,
            _                     => throw new ArgumentOutOfRangeException(nameof(ConeOrigin)),
        };

        public override SdfData sdfData => new()
        {
            evaluationExpression = p => FunctionCall(controllerIdentifier, p.evaluationExpression),
            Requirements = new API.Data.Requirement[]
            {
                new HlslIncludeFileRequirement("Packages/me.tooster.sdf/Editor/Resources/Includes/primitives.hlsl"),
                createSdfFunctionRequirement(
                    p => new ScalarData
                    {
                        evaluationExpression = FunctionCall(
                            "sdf::primitives3D::cone",
                            p.evaluationExpression,
                            new Identifier { id = this[coneAngle].identifier },
                            new Identifier { id = this[coneHeight].identifier }
                        ),
                    }
                ),
            },
        };
    }
}
