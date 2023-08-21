using AST.Syntax;

namespace AST.Shaderlab {
    public abstract record ShaderlabSyntax 
        : SyntaxNode<ShaderlabSyntax, IShaderlabSyntaxOrToken>, IShaderlabSyntaxOrToken;
}
