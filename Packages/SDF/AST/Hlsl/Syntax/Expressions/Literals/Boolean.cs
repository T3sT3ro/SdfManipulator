using System.Text.RegularExpressions;

namespace AST.Hlsl.Syntax.Expressions.Literals {
    public record Boolean : Literal {
        public HlslToken Value { get; set; }
    };
}
