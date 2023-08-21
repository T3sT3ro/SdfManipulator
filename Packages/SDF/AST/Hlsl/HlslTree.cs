using AST.Syntax;

namespace AST.Hlsl {
    public record HlslTree(HlslSyntax Root) : SyntaxTree<HlslSyntax, IHlslSyntaxOrToken>(Root);
}
