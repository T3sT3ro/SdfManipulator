namespace me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor {
    [SyntaxNode] public partial record Ifndef : PreprocessorSyntax {
        public IfndefKeyword ifndefKeyword { get; init; } = new();
        public Identifier    id            { get; init; }
    }
}
