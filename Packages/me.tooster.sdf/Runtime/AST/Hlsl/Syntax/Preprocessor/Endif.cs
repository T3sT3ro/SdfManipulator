namespace me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor {
    [SyntaxNode] public partial record Endif : PreprocessorSyntax {
        public EndIfKeyword endifKeyword { get; init; } = new();
    }
}
