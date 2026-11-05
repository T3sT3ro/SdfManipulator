using me.tooster.sdf.AST.Foo.Tokens;
using Expression = me.tooster.sdf.AST.Syntax.CommonSyntax.Expression<me.tooster.sdf.AST.foo>;

namespace me.tooster.sdf.AST.Foo.Syntax.Expressions {
    [SyntaxNode] public partial record LiteralExpression : Expression {
        public ZeroLiteral zero { get; init; }
    }
}
