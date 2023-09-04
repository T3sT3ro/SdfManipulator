using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators {
    [Syntax] public partial record Unary : Expression {
        private readonly Token<Hlsl> _operatorToken;
        private readonly Expression  _expression;
    }
}
