#nullable enable
namespace me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor {
    [SyntaxNode] public partial record Pragma : PreprocessorSyntax {
        public PragmaKeyword pragmaKeyword { get; init; } = new();
        public TokenString?  tokenString   { get; init; } = new();
    }
}
