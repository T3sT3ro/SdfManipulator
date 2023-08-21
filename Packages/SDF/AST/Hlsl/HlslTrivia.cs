using AST.Syntax;

namespace AST.Hlsl {
    public abstract record HlslTrivia : SyntaxTrivia<HlslToken, HlslSyntax, HlslTrivia, IHlslSyntaxOrToken>;
}
