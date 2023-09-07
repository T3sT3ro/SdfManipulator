using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    [Syntax] public partial record Discard : Statement {
        [Init] private readonly DiscardKeyword _discardKeyword;
        [Init] private readonly SemicolonToken _semicolonToken;
    }
}
