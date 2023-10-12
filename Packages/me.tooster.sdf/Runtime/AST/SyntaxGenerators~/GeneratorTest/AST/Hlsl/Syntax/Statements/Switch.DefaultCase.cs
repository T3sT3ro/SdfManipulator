#nullable enable
using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    public partial record Switch {
        [AstSyntax] public partial record DefaultCase : Syntax<Hlsl> {
            public DefaultKeyword              defaultKeyword { get; init; } = new();
            public ColonToken                  colonToken { get; init; } = new();
            public SyntaxList<Hlsl, Statement> body { get; init; } = new();
        }
    }
}
