using AST.Syntax;

namespace AST.Shaderlab {
    public record ShaderlabTree(ShaderlabSyntax Root) : SyntaxTree<ShaderlabSyntax, IShaderlabSyntaxOrToken>(Root);
}
