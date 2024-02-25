using me.tooster.sdf.AST;
using me.tooster.sdf.AST.Hlsl;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions;
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators;
using me.tooster.sdf.AST.Syntax;
using me.tooster.sdf.AST.Syntax.CommonSyntax;
using me.tooster.sdf.Editor.API;

namespace me.tooster.sdf.Editor.Controllers.Data {
    public record VectorData {
        /// underlying type of the vector
        public Constants.ScalarKind vectorType { get; init; }
        /// the dimension of the vector type
        public uint arity { get; init; }
        /// expression that evaluates the value of the vector type
        public Expression<hlsl> evaluationExpression { get; init; }

        /// returns a HLSL's TypeSyntax node representing this vector type
        public Type typeSyntax => new VectorTypeToken { arity = arity, type = vectorType };
    }
}
