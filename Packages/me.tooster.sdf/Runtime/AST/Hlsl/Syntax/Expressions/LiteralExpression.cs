using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Expressions {
    [SyntaxNode] public partial record LiteralExpression : Expression {
        public Literal<hlsl> literal { get; init; }
    }
}
