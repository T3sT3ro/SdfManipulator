#nullable enable
using AST.Hlsl.Syntax.Expressions;

namespace AST.Hlsl.Syntax.Statements {
    public record Return : Statement {
        public Expression? expression { get; set; }
    }
}