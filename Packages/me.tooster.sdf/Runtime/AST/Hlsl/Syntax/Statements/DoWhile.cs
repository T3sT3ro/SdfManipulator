using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    // do { body } while (test);
    [Syntax] public partial record DoWhile : Statement {
        [Init] private readonly DoKeyword       _doKeyword;
        [Init] private readonly OpenBraceToken  _openBraceToken;
        private readonly        Statement       _body;
        [Init] private readonly CloseBraceToken _closeBraceToken;
        [Init] private readonly WhileKeyword    _whileKeyword;
        [Init] private readonly OpenParenToken  _openParenToken;
        private readonly        Expression      _test;
        [Init] private readonly CloseParenToken _closeParenToken;
        [Init] private readonly SemicolonToken  _semicolonToken;
    }
}
