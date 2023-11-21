namespace me.tooster.sdf.AST.Foo.Syntax.Expressions {
    [SyntaxNode] public partial record LiteralExpression : Expression {
        public ZeroLiteral zero { get; init; }
    }
}
