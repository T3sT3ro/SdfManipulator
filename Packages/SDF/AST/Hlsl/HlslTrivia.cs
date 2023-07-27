using AST.Syntax;

namespace AST.Hlsl {
    public abstract record HlslTrivia : 
        ISyntaxTrivia<HlslToken, HlslSyntax, HlslTrivia, HlslSyntaxOrToken> {
        public HlslToken Token { get; }
    }
}