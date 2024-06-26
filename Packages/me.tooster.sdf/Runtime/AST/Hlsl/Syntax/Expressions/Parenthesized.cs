using Expression = me.tooster.sdf.AST.Syntax.CommonSyntax.Expression<me.tooster.sdf.AST.hlsl>;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Expressions {
    [SyntaxNode] public partial record Parenthesized : Expression {
        public OpenParenToken  openParen  { get; init; } = new();
        public Expression      expression { get; init; }
        public CloseParenToken closeParen { get; init; } = new();
    }
}
