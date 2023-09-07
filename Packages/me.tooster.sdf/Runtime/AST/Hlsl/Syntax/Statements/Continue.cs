using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    [Syntax] public partial record Continue : Statement {
        [Init] private readonly ContinueKeyword _continueKeyword;
        [Init] private readonly SemicolonToken  _semicolonToken;
    }
}
