namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    [SyntaxNode] public partial record Continue : Statement {
        public ContinueKeyword continueKeyword { get; init; } = new();
        public SemicolonToken  semicolonToken  { get; init; } = new();
    }
}
