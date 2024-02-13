using me.tooster.sdf.AST;
using me.tooster.sdf.AST.Hlsl;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Syntax.CommonSyntax;

namespace me.tooster.sdf.Editor.Controllers.Data {
    public record VectorData {
        public Constants.ScalarKind VectorType           { get; init; }
        public uint                 Arity                { get; init; }
        /// expression that evaluates the value of the vector type
        public Expression<hlsl>     EvaluationExpression { get; init; }

        public Type TypeSyntax => new VectorTypeToken { arity = 3, type = VectorType };

        /// Expression used to evaluate the vector value. For example "p", "p.xyz" or "float2(1.0, 1.0)"
        public static VectorData Default(Constants.ScalarKind vectorType) => new VectorData { VectorType = vectorType };
    }
}
