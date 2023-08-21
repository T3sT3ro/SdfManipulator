using AST.Syntax;

namespace AST.Shaderlab {
    public abstract class ShaderlabSyntaxWalker
        : SyntaxWalker<ShaderlabSyntax, ShaderlabToken, IShaderlabSyntaxOrToken, ShaderlabTrivia> { }
}
