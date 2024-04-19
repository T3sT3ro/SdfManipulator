#nullable enable
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST;
using me.tooster.sdf.AST.Hlsl;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Syntax.CommonSyntax;
using Type = me.tooster.sdf.AST.Hlsl.Syntax.Type;

namespace me.tooster.sdf.Editor.Controllers.Data {
    /// Represents Signed Distance Field with relevant informations, such as required function declaration or a way of evaluating it.
    public record SdfData : Editor.API.Data {
        static readonly HlslIncludeFileRequirement typeDefinitionsIncludeRequirement =
            new("Packages/me.tooster.sdf/Editor/Resources/Includes/raymarching.hlsl");

        public override IEnumerable<Requirement> Requirements {
            get => base.Requirements.Append(typeDefinitionsIncludeRequirement);
            init => base.Requirements = value;
        }

        /// type returned by SdfFunction
        public static readonly Type.UserDefined sdfReturnType = new() { id = "SdfResult" };
        public const string pParamName = "p";

        public static readonly VectorData pData = new()
        {
            scalarType = Constants.ScalarKind.@float,
            arity = 3,
            evaluationExpression = new Identifier { id = pParamName },
        };

        public delegate Expression<hlsl> EvaluationExpressionFactory(VectorData vectorData);

        /// <summary>Expression to evaluate this sdf at a given point</summary>
        /// <returns>Expresion for evaluation this sdf</returns>
        public EvaluationExpressionFactory? evaluationExpression { get; init; }
    }
}
