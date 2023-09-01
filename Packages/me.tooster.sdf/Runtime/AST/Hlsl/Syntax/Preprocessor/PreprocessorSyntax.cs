using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor {
    public abstract partial record PreprocessorSyntax : Syntax<Hlsl> {
        private readonly HashToken /*_*/hashToken;
    }
}
