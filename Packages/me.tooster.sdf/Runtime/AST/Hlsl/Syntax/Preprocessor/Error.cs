namespace me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor {
    [SyntaxNode] public partial record Error : PreprocessorSyntax {
        public ErrorKeyword errorKeyword { get; init; } = new();
        public TokenString  tokenstring  { get; init; } = new();
    }
}
