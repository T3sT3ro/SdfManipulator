using System;
using me.tooster.sdf.AST.Hlsl;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions;
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators;
using me.tooster.sdf.Editor.Controllers.Data;
using me.tooster.sdf.Editor.Util.Controllers;
using Unity.Properties;
using UnityEngine;
using static me.tooster.sdf.AST.Hlsl.Extensions;

namespace me.tooster.sdf.Editor.Controllers.SDF.Primitives {
    [GeneratePropertyBag]
    public partial class SdfConeController : SdfPrimitiveController {
        public enum OriginPosition {
            [InspectorName("Origin at base")]    BOTTOM,
            [InspectorName("Origin at the tip")] TIP,
        }



        static readonly PropertyPath coneAngleProperty  = new(nameof(Angle));
        static readonly PropertyPath coneHeightProperty = new(nameof(Height));

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

        VectorData transformOrigin(VectorData vd) {
            switch (ConeOrigin) {
                // generate expression returning shifted coordinate frame
                case OriginPosition.BOTTOM:
                    var zeroLiteral = (LiteralExpression)0;
                    return vd with
                    {
                        evaluationExpression = new Parenthesized
                        {
                            expression = new Binary
                            {
                                left = vd.evaluationExpression,
                                operatorToken = new MinusToken(),
                                right = HlslExtensions.VectorConstructor(
                                    Constants.ScalarKind.@float,
                                    3,
                                    zeroLiteral,
                                    (Identifier)this[coneHeightProperty].identifier,
                                    zeroLiteral
                                ),
                            },
                        },
                    };
                case OriginPosition.TIP: return vd;
                default:                 throw new ArgumentOutOfRangeException(nameof(ConeOrigin));
            }
        }

        public override ScalarData Apply(VectorData input, Processor processor) {
            processor.HandleRequirement(new IncludeRequirement(this, "Packages/me.tooster.sdf/Editor/Resources/Includes/primitives.hlsl"));

            var transformedOrigin = transformOrigin(input);
            return new ScalarData
            {
                evaluationExpression = FunctionCall(
                    "sdf::primitives3D::cone",
                    transformedOrigin.evaluationExpression,
                    (Identifier)this[coneAngleProperty].identifier,
                    (Identifier)this[coneHeightProperty].identifier
                ),
            };
        }
    }
}
