using AST.Syntax;

namespace AST.Hlsl.Syntax.Preprocessor {
    public abstract partial record PreprocessorSyntax : Syntax<Hlsl> {
        private readonly HashToken _hashToken;
    }
}
