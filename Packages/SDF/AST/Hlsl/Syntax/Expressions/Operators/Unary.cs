namespace AST.Hlsl.Syntax.Expressions.Operators {
    public record Unary : Expression {
        public enum Kind {
            MINUS, PLUS, LOGICAL_NOT, BIT_NOT
        }

        public Kind       kind       { get; set; }
        public Expression expression { get; set; }
    }
}