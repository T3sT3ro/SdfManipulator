using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators {
    // a, b
    [Syntax] public partial record Comma : Expression {
        private readonly Expression _left;
        [Init] private readonly CommaToken _comma;
        private readonly Expression _right;
    }
}
