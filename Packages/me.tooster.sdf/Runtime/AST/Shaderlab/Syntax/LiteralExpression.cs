using me.tooster.sdf.AST.Syntax;
namespace me.tooster.sdf.AST.Shaderlab.Syntax {
    [SyntaxNode] public partial record LiteralExpression : Syntax<shaderlab> {
        public Literal<shaderlab> literal { get; init; }

        public static implicit operator LiteralExpression(Literal<shaderlab> literal) => new() { literal = literal };
        public static implicit operator LiteralExpression(int value)                  => new() { literal = (IntLiteral)value };
        public static implicit operator LiteralExpression(bool value)                 => new() { literal = (BooleanLiteral)value };
        public static implicit operator LiteralExpression(float value)                => new() { literal = (FloatLiteral)value };
    }
}
