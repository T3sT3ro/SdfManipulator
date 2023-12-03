namespace me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor {
    [SyntaxNode] public partial record If : PreprocessorSyntax {
        public IfKeyword   ifKeyword { get; init; } = new();
        public TokenString condition { get; init; }
    }
}
