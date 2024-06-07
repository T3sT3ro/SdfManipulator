#nullable enable
using System;
using System.Linq;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions;
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators;
using me.tooster.sdf.AST.Hlsl.Syntax.Statements;
using me.tooster.sdf.Editor.Controllers.Data;
using me.tooster.sdf.Editor.Util.Controllers;
using Unity.Properties;
using UnityEngine;
namespace me.tooster.sdf.Editor.Controllers.SDF {
    [GeneratePropertyBag]
    public partial class SdfController : Controller, IModifier<VectorData, SdfData> {
        [SerializeField] [DontCreateProperty] bool inverted = false;

        /// Should the sdf be inverted, i.e. "inside out"?
        [CreateProperty] [ShaderStructural]
        public bool Inverted {
            get => inverted;
            set => SetField(ref inverted, value, true);
        }

        public Controller[] sdfModifiers = Array.Empty<Controller>();

        public virtual SdfData Apply(VectorData input, Processor processor) {
            processor.HandleRequirement(SdfData.includeRaymarchingRequirement(this));

            var sdfModifierStack = ModifierStack.Compose<VectorData, ScalarData>(sdfModifiers.OfType<IModifier>().ToArray());
            var evalSdfData = (ScalarData)sdfModifierStack.Apply(SdfData.pData, processor);

            var sdfFunctionName = SdfScene.sceneData.controllers[this].identifier;
            /*
            Generates a unique function of signature <c>(p: float3) -> SdfResult</c> for evaluating a primitive.
            The generated function assigns unique primitive ID and uses <c>sdfPrimitive</c> to evaluate SDF at point <c>p</c>.
            and returning distance (scalar data).
            */
            processor.HandleRequirement(
                new FunctionDefinitionRequirement(
                    this,
                    SdfData.createSdfFunction(
                        (Identifier)sdfFunctionName,
                        new[]
                        {
                            // SdfResult result = (SdfResult) 0;
                            AST.Hlsl.Extensions.Var(
                                SdfData.sdfReturnType,
                                "result",
                                new Cast { type = SdfData.sdfReturnType, expression = (LiteralExpression)0 }
                            ),
                            // result.distance = [-]<(vector) -> scalar expression of p>
                            AST.Hlsl.Extensions.Assignment(
                                $"result.{SdfData.sdfResultDistanceMemberName}",
                                Inverted
                                    ? new Unary
                                    {
                                        operatorToken = new MinusToken(),
                                        expression = evalSdfData.evaluationExpression,
                                    }
                                    : evalSdfData.evaluationExpression
                            ),
                            // result.id = <primitiveID integer>;
                            AST.Hlsl.Extensions.Assignment(
                                $"result.{SdfData.sdfResultIdMemberName}",
                                HlslExtensions.VectorConstructor(0, 0, 0, SdfScene.sceneData.controllers[this].numericId)
                            ),
                            // return result;
                            (Return)new Identifier { id = "result" },
                        }
                    )
                )
            );

            return new SdfData { evaluationExpression = AST.Hlsl.Extensions.FunctionCall(sdfFunctionName, input.evaluationExpression) };
        }
    }
}
