using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor {
    [AstSyntax] public abstract partial record PreprocessorSyntax : Syntax<Hlsl> {
        public HashToken hashToken { get; init; } = new();
    }
}
