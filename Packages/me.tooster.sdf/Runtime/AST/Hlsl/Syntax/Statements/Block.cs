using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    public partial record Block : Statement {
        private readonly OpenBraceToken              /*_*/openBraceToken;
        private readonly SyntaxList<Hlsl, Statement> /*_*/statements;
        private readonly CloseBraceToken             /*_*/closeBraceToken;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { openBraceToken, statements, closeBraceToken };
    }
}
