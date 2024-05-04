using System.Linq;
using me.tooster.sdf.AST.Hlsl;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions;
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators;
using me.tooster.sdf.AST.Hlsl.Syntax.Statements;
using me.tooster.sdf.Editor.Controllers.Data;
using Unity.Properties;
using UnityEngine;
using FunctionDefinition = me.tooster.sdf.AST.Hlsl.Syntax.Statements.FunctionDefinition;
namespace me.tooster.sdf.Editor.Controllers.SDF {
    [GeneratePropertyBag]
    public partial class SdfElongate : Controller, ISdfDataSource {
        [SerializeField] [DontCreateProperty] Vector3 length;

        [CreateProperty] [ShaderProperty(Description = "Elongation")]
        Vector3 Length {
            get => length;
            set => SetField(ref length, value, false);
        }

        public SdfData sdfData {
            get {
                var elongatedSdfData = GetNextControllerInStack<SdfController>().sdfData;
                var functionDefinition = elongatedSdf(elongatedSdfData);
                return new SdfData
                {
                    evaluationExpression = vd => AST.Hlsl.Extensions.FunctionCall(controllerIdentifier, vd.evaluationExpression),
                    Requirements = elongatedSdfData.Requirements.Concat(
                        new API.Data.Requirement[]
                        {
                            new HlslIncludeFileRequirement("Packages/me.tooster.sdf/Editor/Resources/Includes/operators.hlsl"),
                            new HlslIncludeFileRequirement("Packages/me.tooster.sdf/Editor/Resources/Includes/util.hlsl"),
                            new HlslFunctionRequirement
                            {
                                functionIdentifier = controllerIdentifier,
                                functionDefinition = functionDefinition,
                            },
                        }
                    ),
                };
            }
        }

        const string qDataName = "q";
        static VectorData qData = new()
            { scalarType = Constants.ScalarKind.@float, arity = 3, evaluationExpression = (Identifier)qDataName };

        FunctionDefinition elongatedSdf(SdfData elongatedSdfData) {
            return SdfData.createSdfFunction(
                controllerIdentifier,
                new[]
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
                    // SdfResult = <some primitive>(abs(q,0))'
                    AST.Hlsl.Extensions.Var(
                        SdfData.sdfReturnType,
                        "result",
                        elongatedSdfData.evaluationExpression(
                            qData with
                            {
                                evaluationExpression = AST.Hlsl.Extensions.FunctionCall(
                                    "max",
                                    qData.evaluationExpression,
                                    (LiteralExpression)0
                                ),
                            }
                        )
                    ),
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
                }
            );
        }
    }
}
