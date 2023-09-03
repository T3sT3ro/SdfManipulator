using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Expressions {
    [Syntax] public partial record Parenthesized : Expression {
        [Init] private readonly OpenParenToken  _openParen;
        private readonly        Expression      _expression;
        [Init] private readonly CloseParenToken _closeParen;
    }
}
