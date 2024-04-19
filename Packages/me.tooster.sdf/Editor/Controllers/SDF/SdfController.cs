#nullable enable
using System;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions;
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators;
using me.tooster.sdf.AST.Hlsl.Syntax.Statements;
using me.tooster.sdf.AST.Hlsl.Syntax.Statements.Definitions;
using me.tooster.sdf.Editor.Controllers.Data;
using me.tooster.sdf.Editor.Util;
using Unity.Properties;
using UnityEngine;
namespace me.tooster.sdf.Editor.Controllers.SDF {
    // TODO: use this interface instead of the concrete classes
    public interface ISdfDataSource {
        public SdfData sdfData { get; }
    }



    [DisallowMultipleComponent]
    public abstract partial class SdfController : SdfTransformController, ISdfDataSource {
        [SerializeField] [DontCreateProperty] bool inverted = false;

        /// Should the sdf be inverted, i.e. "inside out"?
        [CreateProperty] [ShaderStructural]
        public bool Inverted {
            get => inverted;
            set => SetField(ref inverted, value, true);
        }

        // TODO: remove this, use IdentifierRequirement instead
        protected string sdfFunctionIdentifier => SdfScene.sceneData.controllers[this].identifier;


        protected override void Update() {
            base.Update();
            if (!transform.hasChanged) return;
            var tr = transform;
            tr.localScale = Vector3.one;
        }

        /// <summary>
        /// Returns the sdfData for evaluating this primitive. It can be inline or use the primitive function
        /// </summary>
        /// <seealso cref="createSdfFunctionRequirement"/>
        public abstract SdfData sdfData { get; }

        /// <summary>
        /// Generates a unique primitive function of signature <c>(float3) -> SdfResult</c>.
        /// </summary>
        /// <param name="sdfPrimitive">A transformation from vector data at point of the fild into the signed distance</param>
        /// <returns>Create function definition <c>(float3) -> SdfResult</c></returns>
        protected HlslFunctionRequirement createSdfFunctionRequirement(Func<VectorData, ScalarData> sdfPrimitive) {
            if (sdfPrimitive == null)
                throw new ArgumentNullException(nameof(sdfPrimitive), "Required distance data for calculating distance is missing");

            return new HlslFunctionRequirement
            {
                requiredFunction = createSdfFunction(
                    sdfFunctionIdentifier,
                    new[]
                    {
                        AST.Hlsl.Extensions.Var(
                            SdfData.sdfReturnType,
                            "result",
                            new Cast { type = SdfData.sdfReturnType, expression = (LiteralExpression)0 }
                        ),
                        AST.Hlsl.Extensions.Assignment(
                            SdfData.pParamName,
                            TransformVectorData(SdfData.pData).evaluationExpression
                        ),
                        AST.Hlsl.Extensions.Assignment(
                            $"result.{SdfData.sdfResultDistanceMemberName}",
                            Inverted
                                ? new Unary
                                {
                                    operatorToken = new MinusToken(),
                                    expression = sdfPrimitive(SdfData.pData).evaluationExpression,
                                }
                                : sdfPrimitive(SdfData.pData).evaluationExpression
                        ),
                        AST.Hlsl.Extensions.Assignment(
                            $"result.{SdfData.sdfResultIdMemberName}",
                            HlslExtensions.VectorConstructor(0, 0, 0, SdfScene.sceneData.controllers[this].numericId)
                        ),
                        (Return)new Identifier { id = "result" },
                    }
                ),
            };
        }

        protected FunctionDefinition createSdfFunction(Identifier identifier, Block functionBody)
            => new()
            {
                returnType = SdfData.sdfReturnType,
                id = identifier,
                paramList = new FunctionDefinition.Parameter { type = SdfData.pData.typeSyntax, id = SdfData.pParamName },
                body = functionBody,
            };
    }
}
