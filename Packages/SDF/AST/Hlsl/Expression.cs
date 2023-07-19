#nullable enable
namespace AST.Hlsl {
    public interface Expression {
        public record Literal : Expression {
            public string value { get; set; }
        }

        public record Call : Expression {
            public Identifier   id        { get; set; }
            public Expression[] arguments { get; set; }
        }
        
        public interface Operator {
            public record Unary : Expression {
                public enum Kind {
                    MINUS, PLUS, LOGICAL_NOT, BIT_NOT
                }

                public Kind       kind       { get; set; }
                public Expression expression { get; set; }
            }

            public record Binary : Expression {
                public enum Kind {
                    PLUS, MINUS, MUL, DIV, MOD,
                    BIT_L_SHIFT, BIT_R_SHIFT, BIT_AND, BIT_OR, BIT_XOR,
                    CMP_LESS, CMP_LESS_EQ, CMP_GREATER, CMP_GREATER_EQ, CMP_EQ, CMP_NOT_EQ,
                }

                public Expression left  { get; set; }
                public Kind       kind  { get; set; }
                public Expression right { get; set; }
            }

            public record Ternary : Expression {
                public Expression condition { get; set; }
                public Expression whenTrue  { get; set; }
                public Expression whenFalse { get; set; }
            }

            public record Indexer : Expression {
                public Expression expression { get; set; }
                public Expression index      { get; set; }
            }

            public record Assignment : Expression, Statement.For.Initializer {
                public enum Kind {
                    ASSIGN, ADD_ASSIGN, SUB_ASSIGN, MUL_ASSIGN, DIV_ASSIGN, MOD_ASSIGN,
                    BIT_L_SHIFT_ASSIGN, BIT_R_SHIFT_ASSIGN, BIT_AND_ASSIGN, BIT_OR_ASSIGN, BIT_XOR_ASSIGN,
                }

                public interface Left { }

                public Left       left  { get; set; }
                public Kind       kind  { get; set; }
                public Expression right { get; set; }
            }

            public record Member : Assignment.Left, Expression {
                public Expression expression { get; set; }
                public Identifier member     { get; set; }
            }

            public record Cast : Expression {
                public Type       type       { get; set; }
                public Expression expression { get; set; }
            }

            public record Prefix : Expression {
                public enum Kind { PRE_INC, PRE_DEC }

                public Identifier id   { get; set; }
                public Kind       kind { get; set; }
            }

            public record Postfix : Expression {
                public enum Kind { POST_INC, POST_DEC }

                public Identifier id   { get; set; }
                public Kind       kind { get; set; }
            }

            public record Comma : Expression {
                public Expression left  { get; set; }
                public Expression right { get; set; }
            }
        }
    }
}
