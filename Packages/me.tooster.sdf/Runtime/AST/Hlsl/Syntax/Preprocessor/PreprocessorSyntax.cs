using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor {
    [SyntaxNode] public abstract partial record PreprocessorSyntax : Syntax<hlsl> {
        public HashToken     hashToken           { get; init; } = new();
        public LineFeedToken endOfDirectiveToken { get; init; } = new();
    }
}
