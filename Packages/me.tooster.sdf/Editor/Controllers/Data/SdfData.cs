using me.tooster.sdf.AST;
using me.tooster.sdf.AST.Hlsl;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Syntax.CommonSyntax;
using Type = me.tooster.sdf.AST.Hlsl.Syntax.Type;

namespace me.tooster.sdf.Editor.Controllers.Data {
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
    public record SdfData : IData {
        public static IncludeRequirement includeRaymarchingRequirement(IModifier originator)
            => new(originator, "Packages/me.tooster.sdf/Editor/Resources/Includes/raymarching.hlsl");

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

        /// <summary>Expression to evaluate this sdf at a given point</summary>
        /// <returns>Expresion for evaluation this sdf</returns>
        public Expression<hlsl> evaluationExpression { get; init; }
    }
}
