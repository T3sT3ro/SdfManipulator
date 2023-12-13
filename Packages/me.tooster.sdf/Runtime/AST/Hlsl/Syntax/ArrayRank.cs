using me.tooster.sdf.AST.Hlsl.Syntax.Expressions;

namespace me.tooster.sdf.AST.Hlsl.Syntax {
    [SyntaxNode] public partial record ArrayRank {
        public OpenBracketToken  openBracketToken  { get; init; } = new();
        public LiteralExpression dimension         { get; init; }
        public CloseBracketToken closeBracketToken { get; init; } = new();
    }
}
