using me.tooster.sdf.AST.Syntax;
using Expression = me.tooster.sdf.AST.Syntax.CommonSyntax.Expression<me.tooster.sdf.AST.hlsl>;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators {
    [SyntaxNode] public partial record Unary : Expression {
        public Token<hlsl> operatorToken { get; init; }
        public Expression  expression    { get; init; }
    }
}
