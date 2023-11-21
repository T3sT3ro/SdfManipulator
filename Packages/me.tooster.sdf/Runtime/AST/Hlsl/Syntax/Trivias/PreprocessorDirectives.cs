#nullable enable
using me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Trivias {
    public abstract partial record PreprocessorDirective : StructuredTrivia<hlsl> {
        public PreprocessorSyntax triviaSyntax { get; init; }
    }

    // used to break preprocessor lines
    public partial record PreprocessorLineConcatTrivia : SimpleTrivia<hlsl> { }
}
