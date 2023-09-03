#nullable enable
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    // if (test) then
    // if (test) then else elseStatement
    [Syntax] public partial record If : Statement {
        [Init] private readonly IfKeyword       _ifKeyword;
        [Init] private readonly OpenParenToken  _openParen;
        private readonly        Expression      _test;
        [Init] private readonly CloseParenToken _closeParen;
        private readonly        Statement       _then;
        private readonly        ElseClause?     _elseClause;
    }
}
