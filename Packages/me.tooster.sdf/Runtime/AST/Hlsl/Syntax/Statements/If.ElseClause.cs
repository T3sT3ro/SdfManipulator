#nullable enable
using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    public partial record If {
        [Syntax] public partial record ElseClause : Syntax<Hlsl> {
            [Init] private readonly ElseKeyword _elseKeyword;
            private readonly        Statement   _statement;
        }
    }
}
