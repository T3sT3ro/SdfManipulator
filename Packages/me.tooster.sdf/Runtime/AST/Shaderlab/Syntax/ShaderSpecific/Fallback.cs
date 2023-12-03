namespace me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific {
    // Fallback "shader"
    [SyntaxNode] public partial record Fallback : ShaderStatement {
        public FallbackKeyword     fallbackKeyword { get; init; } = new();
        public QuotedStringLiteral name            { get; init; }
    }
}
