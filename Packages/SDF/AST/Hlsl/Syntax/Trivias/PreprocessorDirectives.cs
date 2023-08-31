#nullable enable
using AST.Hlsl.Syntax.Preprocessor;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Trivias {
    public abstract partial record PreprocessorDirective : StructuredTrivia<Hlsl> {
        private readonly PreprocessorSyntax _triviaSyntax;
    }

    // used to break preprocessor lines
    public partial record PreprocessorLineConcatTrivia : Trivia<Hlsl> { }
}
