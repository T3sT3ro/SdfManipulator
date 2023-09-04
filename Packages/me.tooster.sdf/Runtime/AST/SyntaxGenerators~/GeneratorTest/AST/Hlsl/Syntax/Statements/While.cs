using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    [Syntax] public partial record While : Statement {
        [Init] private readonly WhileKeyword    _whileKeyword;
        [Init] private readonly OpenParenToken  _openParen;
        private readonly        Expression      _test;
        [Init] private readonly CloseParenToken _closeParen;
        private readonly        Statement       _body;
    }
}
