using Expression = me.tooster.sdf.AST.Syntax.CommonSyntax.Expression<me.tooster.sdf.AST.hlsl>;
using Statement = me.tooster.sdf.AST.Syntax.CommonSyntax.Statement<me.tooster.sdf.AST.hlsl>;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    // do { body } while (test);
    [SyntaxNode] public partial record DoWhile : Statement {
        public DoKeyword       doKeyword       { get; init; } = new();
        public OpenBraceToken  openBraceToken  { get; init; } = new();
        public Statement       body            { get; init; }
        public CloseBraceToken closeBraceToken { get; init; } = new();
        public WhileKeyword    whileKeyword    { get; init; } = new();
        public OpenParenToken  openParenToken  { get; init; } = new();
        public Expression      test            { get; init; }
        public CloseParenToken closeParenToken { get; init; } = new();
        public SemicolonToken  semicolonToken  { get; init; } = new();
    }
}
