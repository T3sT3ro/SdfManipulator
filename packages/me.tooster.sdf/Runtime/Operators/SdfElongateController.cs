using me.tooster.sdf.AST;
using me.tooster.sdf.AST.Hlsl;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions;
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators;
using me.tooster.sdf.AST.Hlsl.Syntax.Statements;
using me.tooster.sdf.AST.Syntax.CommonSyntax;
using me.tooster.sdf.Data;
using me.tooster.sdf.Primitives;
using me.tooster.sdf.Util;
using Unity.Properties;
using UnityEngine;
using Extensions = me.tooster.sdf.AST.Hlsl.Extensions;
namespace me.tooster.sdf.Operators {
    /*
     * https://iquilezles.org/articles/distfunctions/
     * Elongates an Sdf by performing the following operation
     *  elongate(sdf3d primitive, vec3p, vec3 h):
     *      vec3 q = abs(p)-h;
     *      return primitive(max(q, 0.0)) + min(max(q.x, q.y, q.z)), 0.0);
     */
    [GeneratePropertyBag]
    public partial class SdfElongateController : SdfPrimitiveController {
        const string qDataName = "q";

        static readonly VectorData qData = new()
            { scalarType = Constants.ScalarKind.@float, arity = 3, evaluationExpression = (Identifier)qDataName };
        [SerializeField] [DontCreateProperty]
        Vector3 length;

        [ShaderStructural] public Controller sdfPrimitive;

        [CreateProperty]
        [ShaderProperty(Description = "Elongation")]
        public Vector3 Length {
            get => length;
            set => SetField(ref length, value, false);
        }

        protected override void OnValidate() {
            base.OnValidate();
            if (sdfPrimitive == null || sdfPrimitive is not SdfPrimitiveController)
                Debug.LogError("sdf elongate requires an sdf controller as a target!", this);

            // TODO: support elongating combinations i.e. SdfData returning functions as well
        }

        public override ScalarData Apply(VectorData input, Processor processor) {
            processor.HandleRequirement(
                new IncludeRequirement(
                    this,
                    SdfPaths.ShaderLibrary.Operators
                )
            );
            processor.HandleRequirement(
                new IncludeRequirement(
                    this,
                    SdfPaths.ShaderLibrary.Util
                )
            );

            var elongatedFunctionDefinition = createSdfPrimitiveFunction(
                SdfScene.sceneData.controllers[this].identifier,
                new Statement<hlsl>[]
                {
                    // float3 q = abs(p) - h
                    Extensions.Var(
                        qData.typeSyntax,
                        qDataName,
                        new Binary
                        {
                            left = Extensions.FunctionCall("abs", SdfData.pData.evaluationExpression),
                            operatorToken = new MinusToken(),
                            right = (Identifier)this[new PropertyPath(nameof(Length))].identifier,
                        }
                    ),
                    /*// SdfResult = <some primitive>(abs(q,0))'
                    AST.Hlsl.Extensions.Var(
                        SdfData.sdfReturnType,
                        "result",
                        ((SdfPrimitiveController)sdfPrimitive).Apply(
                            qData with
                            {
                                evaluationExpression = AST.Hlsl.Extensions.FunctionCall(
                                    "max",
                                    qData.evaluationExpression,
                                    (LiteralExpression)0
                                ),
                            },
                            processor
                        ).evaluationExpression
                    ),*/
                    // return primitive(max(q, 0.0)) + min(max(q.x, q.y, q.z)), 0.0);
                    (Return)new Binary
                    {
                        left = ((SdfPrimitiveController)sdfPrimitive).Apply(
                            qData with
                            {
                                evaluationExpression = Extensions.FunctionCall(
                                    "max",
                                    qData.evaluationExpression,
                                    (LiteralExpression)0
                                ),
                            },
                            processor
                        ).evaluationExpression,
                        operatorToken = new PlusToken(),
                        right = Extensions.FunctionCall(
                            "min",
                            Extensions.FunctionCall("max", qData.evaluationExpression), // to util.hlsl
                            (LiteralExpression)0
                        ),
                    },

                    /*
                    // result.p = p;
                    AST.Hlsl.Extensions.Assignment($"result.{SdfData.pParamName}", SdfData.pData.evaluationExpression),
                    // result.distance += min(max(q), 0);
                    AST.Hlsl.Extensions.Assignment(
                        $"result.{SdfData.sdfResultDistanceMemberName}",
                        assignmentToken: new PlusEqualsToken(),
                        right: AST.Hlsl.Extensions.FunctionCall(
                            "min",
                            AST.Hlsl.Extensions.FunctionCall("max", qData.evaluationExpression), // to util.hlsl
                            (LiteralExpression)0
                        )
                    ),
                    // return result;
                    (Return)new Identifier { id = "result" },
                    */
                }
            );
            processor.HandleRequirement(new FunctionDefinitionRequirement(this, elongatedFunctionDefinition));

            return new ScalarData
            {
                evaluationExpression =
                    Extensions.FunctionCall(
                        elongatedFunctionDefinition.id.id.Text,
                        input.evaluationExpression
                        // (Identifier)this[new PropertyPath(nameof(Length))].identifier
                    ),
            };
        }
    }
}
