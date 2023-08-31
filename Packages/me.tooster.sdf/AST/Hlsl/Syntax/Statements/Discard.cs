using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Statements {
    public partial record Discard : Statement {
        private readonly DiscardKeyword _discardKeyword;
        private readonly SemiToken      _semiToken;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new Token<Hlsl>[]
            { discardKeyword, semiToken };
    }
}
