using me.tooster.sdf.AST.Hlsl.Syntax.Expressions;
using Expression = me.tooster.sdf.AST.Syntax.CommonSyntax.Expression<me.tooster.sdf.AST.hlsl>;
using Statement = me.tooster.sdf.AST.Syntax.CommonSyntax.Statement<me.tooster.sdf.AST.hlsl>;

namespace me.tooster.sdf.AST.Hlsl.Syntax {
    [SyntaxNode] public partial record ArrayRank {
        public OpenBracketToken  openBracketToken  { get; init; } = new();
        public LiteralExpression dimension         { get; init; }
        public CloseBracketToken closeBracketToken { get; init; } = new();
    }
}
