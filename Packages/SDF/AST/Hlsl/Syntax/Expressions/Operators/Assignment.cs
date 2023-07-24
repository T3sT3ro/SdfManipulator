using AST.Hlsl.Syntax.Statements;

namespace AST.Hlsl.Syntax.Expressions.Operators {
    public record Assignment : Expression, For.Initializer {
        public enum Kind {
            ASSIGN, ADD_ASSIGN, SUB_ASSIGN, MUL_ASSIGN, DIV_ASSIGN, MOD_ASSIGN,
            BIT_L_SHIFT_ASSIGN, BIT_R_SHIFT_ASSIGN, BIT_AND_ASSIGN, BIT_OR_ASSIGN, BIT_XOR_ASSIGN,
        }

        public interface Left { }

        public Left       left  { get; set; }
        public Kind       kind  { get; set; }
        public Expression right { get; set; }
    }
}