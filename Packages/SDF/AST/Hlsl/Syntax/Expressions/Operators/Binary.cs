namespace AST.Hlsl.Syntax.Expressions.Operators {
    public record Binary : Expression {
        public enum Kind {
            PLUS, MINUS, MUL, DIV, MOD,
            BIT_LSHIFT, BIT_RSHIFT, BIT_AND, BIT_OR, BIT_XOR,
            CMP_LESS, CMP_LESS_EQ, CMP_GREATER, CMP_GREATER_EQ, CMP_EQ, CMP_NOT_EQ,
            LOGICAL_AND, LOGICAL_OR,
        }

        public Expression left  { get; set; }
        public Kind       kind  { get; set; }
        public Expression right { get; set; }
    }
}