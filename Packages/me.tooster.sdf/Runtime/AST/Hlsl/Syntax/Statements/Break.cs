using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    [Syntax] public partial record Break : Statement {
        [Init] private readonly BreakKeyword _breakKeyword;
        [Init] private readonly SemicolonToken    _semicolonToken;
    }
}
