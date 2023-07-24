using System.Collections.Generic;
using AST.Hlsl.Syntax.Expressions;

namespace AST.Hlsl.Syntax.Statements {
    public record While : Statement {
        public Expression               test { get; set; }
        public IReadOnlyList<Statement> body { get; set; }
    }
}