#nullable enable
using me.tooster.sdf.AST;
using me.tooster.sdf.AST.Hlsl;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions;
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators;
using me.tooster.sdf.AST.Syntax.CommonSyntax;
using Type = me.tooster.sdf.AST.Hlsl.Syntax.Type;

namespace me.tooster.sdf.Editor.Controllers.Data {
    /// Represents Signed Distance Field with relevant informations, such as required function declaration or a way of evaluating it.
    [ShaderInclude("Packages/me.tooster.sdf/Editor/Resources/Includes/raymarching.hlsl")]
    public record SdfData : Editor.API.Data {
        public delegate Expression<hlsl> EvaluationExpressionFactory(VectorData vectorData);
        public const    string           pParamName = "p";
        /// type returned by SdfFunction
        public static readonly Type.UserDefined sdfReturnType = new() { id = "SdfResult" };

        public static readonly SdfData Empty = new()
        {
            evaluationExpression = _ => new Cast { type = sdfReturnType, expression = new LiteralExpression { literal = (IntLiteral)0 } },
        };


        public static readonly VectorData pData = new()
        {
            scalarType = Constants.ScalarKind.@float,
            arity = 3,
            evaluationExpression = new Identifier { id = pParamName },
        };

        /// <summary>Expression to evaluate this sdf at a given point</summary>
        /// <returns>Expresion for evaluation this sdf</returns>
        public EvaluationExpressionFactory? evaluationExpression { get; init; }
    }
}
