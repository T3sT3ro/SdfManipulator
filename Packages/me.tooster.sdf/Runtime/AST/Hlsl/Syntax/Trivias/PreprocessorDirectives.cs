#nullable enable
using me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Trivias {
    public abstract partial record PreprocessorDirective : StructuredTrivia<Hlsl> {
        private readonly PreprocessorSyntax /*_*/triviaSyntax;
    }

    // used to break preprocessor lines
    public partial record PreprocessorLineConcatTrivia : Trivia<Hlsl> { }
}
