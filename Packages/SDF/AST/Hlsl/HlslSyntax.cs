using AST.Syntax;

namespace AST.Hlsl {
    public abstract record HlslSyntax : SyntaxNode<HlslSyntax, HlslSyntaxOrToken>, HlslSyntaxOrToken;
}
