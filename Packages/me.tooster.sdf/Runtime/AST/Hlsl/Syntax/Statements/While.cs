namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    [SyntaxNode] public partial record While : Statement {
        public WhileKeyword    whileKeyword { get; init; } = new();
        public OpenParenToken  openParen    { get; init; } = new();
        public Expression      test         { get; init; }
        public CloseParenToken closeParen   { get; init; } = new();
        public Statement       body         { get; init; }
    }
}
