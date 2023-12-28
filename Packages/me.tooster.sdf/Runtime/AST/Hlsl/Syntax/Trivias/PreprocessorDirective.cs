#nullable enable
using me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Trivias {
    public partial record PreprocessorDirective : StructuredTrivia<hlsl, PreprocessorSyntax>;
}
