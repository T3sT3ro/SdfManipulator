using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators {
    // a.b.c
    [Syntax] public partial record Member : Expression {
        private readonly        Expression _expression;
        [Init] private readonly DotToken   _dotToken;
        private readonly        Identifier _member;
    }
}
