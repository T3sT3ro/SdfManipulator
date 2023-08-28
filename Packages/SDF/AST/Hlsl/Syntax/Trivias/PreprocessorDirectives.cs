#nullable enable
using AST.Hlsl.Syntax.Preprocessor;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Trivias {
    public abstract record PreprocessorDirective : StructuredTrivia<Hlsl> {
        public PreprocessorSyntax triviaSyntax { get; init; }
    }

    // used to break preprocessor lines
    public record PreprocessorLineConcatTrivia : Trivia<Hlsl> { }
}
