namespace me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor {
    [SyntaxNode] public partial record Ifdef : PreprocessorSyntax {
        public IfdefKeyword ifdefKeyword { get; init; } = new();
        public Identifier   id           { get; init; }
    }
}
