#nullable enable
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    public partial record Switch {
        [Syntax] public partial record Case : Syntax<Hlsl> {
            [Init] private readonly CaseKeyword                 _caseKeyword;
            private readonly        IntLiteral                  _label;
            [Init] private readonly ColonToken                  _colonToken;
            [Init] private readonly SyntaxList<Hlsl, Statement> _body;
        }
    }
}
