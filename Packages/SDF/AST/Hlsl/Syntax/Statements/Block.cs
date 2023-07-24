using System.Collections.Generic;

namespace AST.Hlsl.Syntax.Statements {
    public record Block : Statement {
        public IReadOnlyList<Statement> statements { get; set; }
    }
}
