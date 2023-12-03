namespace me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor {
    [SyntaxNode] public partial record Undef : PreprocessorSyntax {
        public UndefKeyword undefKeyword { get; init; } = new();
        public Identifier   id           { get; init; }
    }
}
