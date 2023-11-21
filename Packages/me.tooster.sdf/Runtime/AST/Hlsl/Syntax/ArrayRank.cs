using me.tooster.sdf.AST.Hlsl.Syntax.Expressions;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax {
    [SyntaxNode] public partial record ArrayRank {
        public OpenBracketToken              openBracketToken  { get; init; } = new();
        public LiteralExpression<IntLiteral> dimension         { get; init; }
        public CloseBracketToken             closeBracketToken { get; init; } = new();
    }
}
