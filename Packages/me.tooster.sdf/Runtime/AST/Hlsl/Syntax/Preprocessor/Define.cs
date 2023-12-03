#nullable enable
namespace me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor {
    [SyntaxNode] public partial record Define : PreprocessorSyntax {
        public DefineKeyword             defineKeyword { get; init; } = new();
        public ArgumentList<Identifier>? argList       { get; init; } = new();
        public Identifier                id            { get; init; }
        public TokenString               tokenString   { get; init; } = new();
    }
}
