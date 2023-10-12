#nullable enable
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    public partial record Switch {
        [AstSyntax] public partial record Case : Syntax<Hlsl> {
            public CaseKeyword                 caseKeyword { get; init; } = new();
            public        IntLiteral                  label { get; init; }
            public ColonToken                  colonToken { get; init; } = new();
            public SyntaxList<Hlsl, Statement> body { get; init; } = new();
        }
    }
}
