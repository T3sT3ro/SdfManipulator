using me.tooster.sdf.AST.Syntax;
using Expression = me.tooster.sdf.AST.Syntax.CommonSyntax.Expression<me.tooster.sdf.AST.hlsl>;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Expressions {
    [SyntaxNode] public partial record LiteralExpression : Expression {
        public Literal<hlsl> literal { get; init; }
    }
}
