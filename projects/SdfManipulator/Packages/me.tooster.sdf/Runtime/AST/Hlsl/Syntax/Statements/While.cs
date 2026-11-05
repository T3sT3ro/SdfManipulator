using Expression = me.tooster.sdf.AST.Syntax.CommonSyntax.Expression<me.tooster.sdf.AST.hlsl>;
using Statement = me.tooster.sdf.AST.Syntax.CommonSyntax.Statement<me.tooster.sdf.AST.hlsl>;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    [SyntaxNode] public partial record While : Statement {
        public WhileKeyword    whileKeyword { get; init; } = new();
        public OpenParenToken  openParen    { get; init; } = new();
        public Expression      test         { get; init; }
        public CloseParenToken closeParen   { get; init; } = new();
        public Statement       body         { get; init; }
    }
}
