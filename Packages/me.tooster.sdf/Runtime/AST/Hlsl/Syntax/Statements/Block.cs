using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    [Syntax] public partial record Block : Statement {
        [Init] private readonly OpenBraceToken              _openBraceToken;
        [Init] private readonly SyntaxList<Hlsl, Statement> _statements;
        [Init] private readonly CloseBraceToken             _closeBraceToken;
    }
}
