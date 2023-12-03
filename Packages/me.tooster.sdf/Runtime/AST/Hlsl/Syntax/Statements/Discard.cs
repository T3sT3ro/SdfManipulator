namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    [SyntaxNode] public partial record Discard : Statement {
        public DiscardKeyword discardKeyword { get; init; } = new();
        public SemicolonToken semicolonToken { get; init; } = new();
    }
}
