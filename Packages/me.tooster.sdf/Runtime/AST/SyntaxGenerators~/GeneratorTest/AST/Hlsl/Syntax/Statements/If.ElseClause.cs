#nullable enable
using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    public partial record If {
        [AstSyntax] public partial record ElseClause : Syntax<Hlsl> {
            public ElseKeyword elseKeyword { get; init; } = new();
            public        Statement   statement { get; init; }
        }
    }
}
