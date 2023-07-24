using System.Collections.Generic;

namespace AST.Hlsl.Syntax.Statements {
    public partial record Switch : Statement {
        public IdentifierName               selector { get; set; }
        public Case[]                   cases    { get; set; }
        public IReadOnlyList<Statement> @default { get; set; }
    }
}