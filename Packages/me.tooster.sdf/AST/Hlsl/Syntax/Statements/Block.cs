using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Statements {
    public partial record Block : Statement {
        private readonly OpenBraceToken              _openBraceToken;
        private readonly SyntaxList<Hlsl, Statement> _statements;
        private readonly CloseBraceToken             _closeBraceToken;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { openBraceToken, statements, closeBraceToken };
    }
}
