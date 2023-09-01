using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    public partial record Continue : Statement {
        private readonly ContinueKeyword /*_*/continueKeyword;
        private readonly SemiToken       /*_*/semiToken;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new Token<Hlsl>[]
            { continueKeyword, semiToken };
    }
}
