using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor {
    [Syntax] public abstract partial record PreprocessorSyntax : Syntax<Hlsl> {
        [Init] private readonly HashToken _hashToken;
    }
}
