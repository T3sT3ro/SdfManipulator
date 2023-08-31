using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Statements {
    public partial record Break : Statement {
        private readonly BreakKeyword _breakKeyword;
        private readonly SemiToken    _semiToken;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new Token<Hlsl>[]
            { breakKeyword, semiToken };
    }
}
