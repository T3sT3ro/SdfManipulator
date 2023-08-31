using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Statements {
    public partial record Continue : Statement {
        private readonly ContinueKeyword _continueKeyword;
        private readonly SemiToken       _semiToken;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new Token<Hlsl>[]
            { continueKeyword, semiToken };
    }
}
