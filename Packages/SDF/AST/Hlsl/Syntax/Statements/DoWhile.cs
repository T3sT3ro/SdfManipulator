using System.Collections.Generic;
using AST.Hlsl.Syntax.Expressions;

namespace AST.Hlsl.Syntax.Statements {
    public record DoWhile : Statement {
        public IReadOnlyList<Statement> body { get; set; }
        public Expression               test { get; set; }
    }
}