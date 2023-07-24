namespace AST.Hlsl.Syntax.Expressions.Operators {
    public record Comma : Expression {
        public Expression left  { get; set; }
        public Expression right { get; set; }
    }
}
