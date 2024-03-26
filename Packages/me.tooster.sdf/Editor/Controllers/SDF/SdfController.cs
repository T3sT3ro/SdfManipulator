#nullable enable
using System;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions;
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators;
using me.tooster.sdf.AST.Hlsl.Syntax.Statements;
using me.tooster.sdf.AST.Hlsl.Syntax.Statements.Definitions;
using me.tooster.sdf.Editor.Controllers.Data;
using UnityEngine;
namespace me.tooster.sdf.Editor.Controllers.SDF {
    [DisallowMultipleComponent]
    public abstract class SdfController : TransformController {
        // TODO: decoupel from TransformController, add dependency and RequiredComponent    
        public enum SdfOrientation {
            /// distance is positive outside the shape and negative inside. Affects unions.
            NORMAL,
            /// distance is negative inside the shape and positive on the outside. Affects unions.
            INSIDE_OUT,
        }

        /// <summary>
        /// The orientation of the sdf, repsective to it's surface, aka if it's "inside out"
        /// </summary>
        public SdfOrientation orientation = SdfOrientation.NORMAL;

        protected override void OnValidate() {
            NotifyStructureChanged(); // hopefully only triggers for "orientation", but it should be handled better later
        }

        /// <summary>
        /// Returns the sdfData for evaluating this primitive. It can be inline or use the primitive function
        /// TODO: handle null SdfData for things like empty unions etc.
        /// </summary>
        /// <seealso cref="generateSdfFunction"/>
        public abstract SdfData sdfData { get; }

        protected Color GizmoColor => orientation == SdfOrientation.NORMAL ? Color.green : Color.red;

        /// <summary>
        /// Generates a unique primitive function of signature <c>(float3) -> SdfResult</c>. 
        /// </summary>
        /// <param name="primitiveDistanceSdfData"></param>
        /// <returns>returns a function for evaluating this primitive's <c>SdfResult</c></returns>
        protected FunctionDefinition generateSdfFunction(SdfData primitiveDistanceSdfData) => new()
        {
            returnType = SdfData.sdfReturnType,
            id = sdfFunctionIdentifier,
            paramList = new FunctionDefinition.Parameter { type = SdfData.sdfArgumentType, id = SdfData.sdfArgumentId },
            body = new[]
            {
                AST.Hlsl.Extensions.Var(SdfData.sdfReturnType, "result", new Cast
                {
                    type = SdfData.sdfReturnType,
                    expression = (LiteralExpression)(IntLiteral)0,
                }),
                AST.Hlsl.Extensions.Assignment(SdfData.sdfArgumentName, ApplyTransform(primitiveFunctionInput).evaluationExpression),
                AST.Hlsl.Extensions.Assignment("result.distance",
                    orientation == SdfOrientation.INSIDE_OUT
                        ? new Unary
                        {
                            operatorToken = new MinusToken(),
                            expression = primitiveDistanceSdfData.evaluationExpression(primitiveFunctionInput),
                        }
                        : primitiveDistanceSdfData.evaluationExpression(primitiveFunctionInput)),
                AST.Hlsl.Extensions.Assignment("result.id", (LiteralExpression)SdfScene.GetControllerId(this)),
                (Return)new Identifier { id = "result" },
            },
        };

        protected string sdfFunctionIdentifier => SdfScene.GetControllerIdentifier(this);

        /// Vector data representing the input of the primitive function, i.e. the <c>p</c> in <c>SdfResult someSdfPrimitive(float3 p) {...}</c>
        private static readonly VectorData primitiveFunctionInput = new()
        {
            arity = SdfData.sdfArgumentType.arity,
            vectorType = SdfData.sdfArgumentType.type,
            evaluationExpression = SdfData.sdfArgumentId,
        };
    }
}
