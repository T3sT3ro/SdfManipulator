namespace me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor {
    [SyntaxNode] public partial record Elif : PreprocessorSyntax {
        public ElifKeyword elifKeyword { get; init; } = new();
        public TokenString condition   { get; init; } = new();
    }
}
