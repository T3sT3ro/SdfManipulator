#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST;
using me.tooster.sdf.AST.Hlsl;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions;
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators;
using me.tooster.sdf.AST.Hlsl.Syntax.Statements;
using me.tooster.sdf.AST.Hlsl.Syntax.Statements.Declarations;
using me.tooster.sdf.AST.Hlsl.Syntax.Statements.Definitions;
using me.tooster.sdf.AST.Syntax.CommonSyntax;
using me.tooster.sdf.Editor.API;
using Type = me.tooster.sdf.AST.Hlsl.Syntax.Type;

namespace me.tooster.sdf.Editor.Controllers.Data {
    /// Represents Signed Distance Field with relevant informations, such as required function declaration or a way of evaluating it.
    [ShaderInclude("Packages/me.tooster.sdf/Editor/Resources/Includes/raymarching.hlsl")]
    public record SdfData : Editor.API.Data {
        /// type returned by SdfFunction
        public static readonly Type.UserDefined sdfReturnType = new() { id = "SdfResult" };

        /// type of the parameter passed to SdfFunction
        public static readonly VectorTypeToken sdfArgumentType = new() { type = Constants.ScalarKind.@float, arity = 3 };

        public static readonly string sdfArgumentName = "p";
        /// name of the parameter used as input to the sdf function.
        public static readonly Identifier sdfArgumentId = new() { id = sdfArgumentName };

        public delegate Expression<hlsl> EvaluationExpressionFactory(VectorData vectorData);
        public delegate Block            FunctionBodyFactory(VectorData sdfPointVectorInputData);

        /// <summary>
        /// SdfData may require some additional function definitions (e.g. for primitive or material).
        /// This holds a list of all required definitions to evaluate this code path.
        /// </summary>
        /// <returns>All required function definition. Can be empty.</returns>
        public IEnumerable<FunctionDefinition> requiredFunctionDefinitions { get; init; } = Enumerable.Empty<FunctionDefinition>();

        /// <summary>Expression to evaluate this sdf at a given point</summary>
        /// <returns>Expresion for evaluation this sdf</returns>
        public EvaluationExpressionFactory? evaluationExpression { get; init; } /* = _ => new Cast
        {
            type = sdfReturnType,
            expression = new LiteralExpression { literal = (IntLiteral)0 },
        };*/

        public static readonly SdfData Empty = new()
        {
            evaluationExpression = _ => new Cast
                { type = sdfReturnType, expression = new LiteralExpression { literal = (IntLiteral)0 } },
        };
    }
}
