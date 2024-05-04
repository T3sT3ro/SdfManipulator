#nullable enable
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST;
using me.tooster.sdf.AST.Hlsl;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Hlsl.Syntax.Statements;
using me.tooster.sdf.AST.Syntax.CommonSyntax;
using FunctionDefinition = me.tooster.sdf.AST.Hlsl.Syntax.Statements.FunctionDefinition;
using Parameter = me.tooster.sdf.AST.Hlsl.Syntax.Parameter;
using Type = me.tooster.sdf.AST.Hlsl.Syntax.Type;

namespace me.tooster.sdf.Editor.Controllers.Data {
#nullable disable
    public interface ISdfDataSource {
        public SdfData sdfData { get; }
    }



#nullable enable
    /// <summary>
    /// <para>Data for for conventional SDF implementation in a shader.</para>
    /// <para>
    /// Sdf data represents:
    /// <list type="bullet">
    ///  <item>requirements needed to satisfy the data evaluation, such as required SDF function definition or include files</item>
    ///  <item>information to build sdf function in hlsl of signature <c>(float3 p) -> SdfResult</c></item>, for example it's return type and parameter type and name
    ///  <item>a factory for evaluation expression of that SDF with respect to VectorData used to evaluate at point <c>p</c></item>
    /// </list>
    /// </para>
    /// </summary>
    public record SdfData : Editor.API.Data {
        static readonly HlslIncludeFileRequirement typeDefinitionsIncludeRequirement =
            new("Packages/me.tooster.sdf/Editor/Resources/Includes/raymarching.hlsl");

        public override IEnumerable<Requirement> Requirements {
            get => base.Requirements.Append(typeDefinitionsIncludeRequirement);
            init => base.Requirements = value;
        }

        /// type returned by SdfFunction
        public static readonly Type.UserDefined sdfReturnType = new() { id = "SdfResult" };
        public const string pParamName                  = "p";
        public const string sdfResultIdMemberName       = "id";
        public const string sdfResultDistanceMemberName = "distance";

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

        public static FunctionDefinition createSdfFunction(Identifier identifier, Block functionBody)
            => new()
            {
                returnType = sdfReturnType,
                id = identifier,
                paramList = new Parameter { type = pData.typeSyntax, id = pParamName },
                body = functionBody,
            };
    }
}
