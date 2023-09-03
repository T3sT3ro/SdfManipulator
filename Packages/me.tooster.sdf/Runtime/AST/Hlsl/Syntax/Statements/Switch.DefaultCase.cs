#nullable enable
using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    public partial record Switch {
        [Syntax] public partial record DefaultCase : Syntax<Hlsl> {
            [Init] private readonly DefaultKeyword              _defaultKeyword;
            [Init] private readonly ColonToken                  _colonToken;
            [Init] private readonly        SyntaxList<Hlsl, Statement> _body;
        }
    }
}
