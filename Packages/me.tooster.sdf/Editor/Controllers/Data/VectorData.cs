using me.tooster.sdf.AST;
using me.tooster.sdf.AST.Hlsl;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Syntax.CommonSyntax;
namespace me.tooster.sdf.Editor.Controllers.Data {
    public record VectorData : IData {
        /// underlying type of the vector
        public Constants.ScalarKind scalarType { get; init; } = Constants.ScalarKind.@float;
        /// the dimension of the vector type
        public uint arity { get; init; } = 4;
        /// expression that evaluates the value of the vector type
        public Expression<hlsl> evaluationExpression { get; init; }

        /// returns a HLSL's TypeSyntax node representing this vector type
        public Type typeSyntax => new VectorTypeToken { arity = arity, type = scalarType };
    }
}
