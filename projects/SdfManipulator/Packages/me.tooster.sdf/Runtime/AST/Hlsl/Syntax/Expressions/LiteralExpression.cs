using me.tooster.sdf.AST.Syntax.CommonSyntax;
using Expression = me.tooster.sdf.AST.Syntax.CommonSyntax.Expression<me.tooster.sdf.AST.hlsl>;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Expressions {
    [SyntaxNode] public partial record LiteralExpression : Expression {
        public Literal<hlsl> literal { get; init; }

        public static implicit operator LiteralExpression(Literal<hlsl> literal) => new() { literal = literal };
        public static implicit operator LiteralExpression(int value)             => new() { literal = (IntLiteral)value };
        public static implicit operator LiteralExpression(bool value)            => new() { literal = (BooleanLiteral)value };
        public static implicit operator LiteralExpression(float value)           => new() { literal = (FloatLiteral)value };
    }
}
