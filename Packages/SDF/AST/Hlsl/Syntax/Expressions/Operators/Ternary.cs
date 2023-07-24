namespace AST.Hlsl.Syntax.Expressions.Operators {
    public record Ternary : Expression {
        public Expression condition { get; set; }
        public Expression whenTrue  { get; set; }
        public Expression whenFalse { get; set; }
    }
}