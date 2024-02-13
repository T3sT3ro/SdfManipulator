#nullable enable
using System;
using me.tooster.sdf.AST;
using me.tooster.sdf.AST.Hlsl;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions;
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators;
using me.tooster.sdf.AST.Hlsl.Syntax.Statements;
using me.tooster.sdf.AST.Hlsl.Syntax.Statements.Definitions;
using me.tooster.sdf.AST.Syntax.CommonSyntax;
using me.tooster.sdf.Editor.API;
using Type = me.tooster.sdf.AST.Hlsl.Syntax.Type;

namespace me.tooster.sdf.Editor.Controllers.Data {
    /// <summary>
    /// This class represent a contract and all necessary context for defining and using an Sdf.
    /// </summary>
    [ShaderInclude("Packages/me.tooster.sdf/Editor/Resources/Includes/raymarching.hlsl")]
    public record SdfData {
        /// type returned by SdfFunction
        public static readonly Type.UserDefined sdfReturnType = new Type.UserDefined { id = "SdfResult" };

        /// type of the parameter passed to SdfFunction
        public static readonly Type.Predefined sdfArgumentType = new VectorTypeToken { type = Constants.ScalarKind.@float, arity = 3 };

        /// name of the parameter used as input to the sdf function.
        public static readonly Identifier sdfArgumentId = new Identifier { id = "p" };

        /// <summary>If this Sdf requires a function definition, this will return that definition.</summary>
        /// <returns>required function definition or a null, if evaluation expression can be inlined without extra function</returns>
        public Func<Identifier, Block, FunctionDefinition?> requiredFunctionDefinition { get; init; } = (functionId, functionBody) =>
            new FunctionDefinition
            {
                returnType = sdfReturnType,
                id = functionId,
                paramList = new FunctionDefinition.Parameter { type = sdfArgumentType, id = sdfArgumentId },
                body = functionBody
            };

        /// <summary>Expression to evaluate this sdf at a given point</summary>
        /// <returns>Expresion for evaluation this sdf</returns>
        public Func<VectorData, Expression<hlsl>> EvaluationExpression { get; init; } =
            (_) => new LiteralExpression { literal = (IntLiteral)0 };

        // TODO: function to indicate that data is not filled yet, or a dependency for slots (function name, param name)
    }
}
