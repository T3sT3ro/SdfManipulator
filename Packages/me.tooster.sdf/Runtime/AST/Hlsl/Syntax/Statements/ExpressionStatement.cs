#nullable enable
using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    // expression;
    // ; <- empty expression statement
    [Syntax] public partial record ExpressionStatement : Statement {
        private readonly        Expression?    _expression;
        [Init] private readonly SemicolonToken _semicolonToken;
    }
}
