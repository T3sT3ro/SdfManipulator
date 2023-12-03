namespace me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators {
    // a, b
    [SyntaxNode] public partial record Comma : Expression {
        public Expression left  { get; init; }
        public CommaToken comma { get; init; } = new();
        public Expression right { get; init; }
    }
}
