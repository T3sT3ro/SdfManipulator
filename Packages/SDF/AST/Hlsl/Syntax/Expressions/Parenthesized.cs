namespace AST.Hlsl.Syntax.Expressions {
    public record Parenthesized : Expression {
        public Expression expression { get; private set; }

        public static Parenthesized From(Expression expr) => new Parenthesized { expression = expr };
    }
}
