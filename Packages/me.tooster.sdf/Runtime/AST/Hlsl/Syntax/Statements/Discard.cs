using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    public partial record Discard : Statement {
        private readonly DiscardKeyword /*_*/discardKeyword;
        private readonly SemiToken      /*_*/semiToken;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new Token<Hlsl>[]
            { discardKeyword, semiToken };
    }
}
