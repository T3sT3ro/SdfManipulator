namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    // = y
    // = {{a}, {b}}}
    [SyntaxNode] public partial record Initializer {
        public EqualsToken equalsToken { get; init; } = new();
        public Expression  value       { get; init; }
    }
}
