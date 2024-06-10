using me.tooster.sdf.AST;
using me.tooster.sdf.AST.Hlsl;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions;
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators;
using me.tooster.sdf.AST.Hlsl.Syntax.Statements;
using me.tooster.sdf.AST.Syntax.CommonSyntax;
using me.tooster.sdf.Editor.Controllers.Data;
using Unity.Properties;
using UnityEngine;
namespace me.tooster.sdf.Editor.Controllers.SDF.Operators {
    /*
     * https://iquilezles.org/articles/distfunctions/
     * Elongates an Sdf by performing the following operation
     *  elongate(sdf3d primitive, vec3p, vec3 h):
     *      vec3 q = abs(p)-h;
     *      return primitive(max(q, 0.0)) + min(max(q.x, q.y, q.z)), 0.0);
     */
    [GeneratePropertyBag]
    public partial class SdfElongateController : SdfPrimitiveController {
        [SerializeField] [DontCreateProperty] Vector3 length;

        [CreateProperty] [ShaderProperty(Description = "Elongation")]
        public Vector3 Length {
            get => length;
            set => SetField(ref length, value, false);
        }

        const string qDataName = "q";
        static VectorData qData = new()
            { scalarType = Constants.ScalarKind.@float, arity = 3, evaluationExpression = (Identifier)qDataName };

        public Controller sdfPrimitive;

        protected override void OnValidate() {
            base.OnValidate();
            if (sdfPrimitive == null || sdfPrimitive is not SdfPrimitiveController)
                Debug.LogError("sdf elongate requires an sdf controller as a target!");

            // TODO: support elongating combinations i.e. SdfData returning functions as well
        }

        public override ScalarData Apply(VectorData input, Processor processor) {
            processor.HandleRequirement(new IncludeRequirement(this, "Packages/me.tooster.sdf/Editor/Resources/Includes/operators.hlsl"));
            processor.HandleRequirement(new IncludeRequirement(this, "Packages/me.tooster.sdf/Editor/Resources/Includes/util.hlsl"));

            var elongatedFunctionDefinition = createSdfPrimitiveFunction(
                SdfScene.sceneData.controllers[this].identifier,
                new Statement<hlsl>[]
                {
                    // float3 q = abs(p) - h
                    AST.Hlsl.Extensions.Var(
                        qData.typeSyntax,
                        name = qDataName,
                        new Binary
                        {
                            left = AST.Hlsl.Extensions.FunctionCall("abs", SdfData.pData.evaluationExpression),
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
                                evaluationExpression = AST.Hlsl.Extensions.FunctionCall(
                                    "max",
                                    qData.evaluationExpression,
                                    (LiteralExpression)0
                                ),
                            },
                            processor
                        ).evaluationExpression,
                        operatorToken = new PlusToken(),
                        right = AST.Hlsl.Extensions.FunctionCall(
                            "min",
                            AST.Hlsl.Extensions.FunctionCall("max", qData.evaluationExpression), // to util.hlsl
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
                    AST.Hlsl.Extensions.FunctionCall(
                        elongatedFunctionDefinition.id.id.Text,
                        input.evaluationExpression
                        // (Identifier)this[new PropertyPath(nameof(Length))].identifier
                    ),
            };
        }
    }
}
