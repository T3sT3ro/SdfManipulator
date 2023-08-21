using AST.Syntax;

namespace AST.Shaderlab {
    public abstract record ShaderlabTrivia
        : SyntaxTrivia<ShaderlabToken, ShaderlabSyntax, ShaderlabTrivia, IShaderlabSyntaxOrToken>;
}
