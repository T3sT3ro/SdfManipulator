#nullable enable
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    // for (start conditions; conditions; increments) { body }
    // start conditions:
    //    declarations: int i, j = 0;
    //    initializers: i = 0, j = 0;
    [Syntax] public partial record For : Statement {
        [Init] private readonly ForKeyword                       _forKeyword;
        [Init] private readonly OpenParenToken                   _openParen;
        private readonly        Initializer?                     _initializer;
        [Init] private readonly SemicolonToken                   _firstSemicolonToken;
        private readonly        Expression?                      _condition;
        [Init] private readonly SemicolonToken                   _secondSemicolonToken;
        [Init] private readonly SeparatedList<Hlsl, Expression>? _increments;
        [Init] private readonly CloseParenToken                  _closeParen;
        private readonly        Statement                        _body;

        [Syntax] public abstract partial record Initializer : Syntax<Hlsl>;
    }
}
