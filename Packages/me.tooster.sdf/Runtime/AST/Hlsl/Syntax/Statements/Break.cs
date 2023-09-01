using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    public partial record Break : Statement {
        private readonly BreakKeyword /*_*/breakKeyword;
        private readonly SemiToken    /*_*/semiToken;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new Token<Hlsl>[]
            { breakKeyword, semiToken };
    }
}
