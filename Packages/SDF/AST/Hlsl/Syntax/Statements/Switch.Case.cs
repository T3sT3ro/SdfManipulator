using System.Collections.Generic;

namespace AST.Hlsl.Syntax.Statements {
    public partial record Switch {
        public record Case : AST.Syntax {
            public uint                     label { get; set; }
            public IReadOnlyList<Statement> body  { get; set; }
        }
    }
}
