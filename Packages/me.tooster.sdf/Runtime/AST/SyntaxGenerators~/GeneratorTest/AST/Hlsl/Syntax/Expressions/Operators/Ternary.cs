using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators {
    [Syntax] public partial record Ternary : Expression {
        private readonly        Expression    _condition;
        [Init] private readonly QuestionToken _questionToken;
        private readonly        Expression    _whenTrue;
        [Init] private readonly ColonToken    _colonToken;
        private readonly        Expression    _whenFalse;
    }
}
