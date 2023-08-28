using AST.Syntax;

namespace AST.Hlsl.Syntax.Preprocessor {
    public abstract record PreprocessorSyntax : Syntax<Hlsl> {
        public HashToken hashToken { get; init; } = new();
    }
}
