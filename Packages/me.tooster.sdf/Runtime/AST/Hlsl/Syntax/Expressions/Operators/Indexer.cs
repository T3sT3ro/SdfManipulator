using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators {
    // something[index]
    public partial record Indexer : Expression {
        private readonly Expression        /*_*/expression;
        private readonly OpenBracketToken  /*_*/openBracketToken;
        private readonly Expression        /*_*/index;
        private readonly CloseBracketToken /*_*/closeBracketToken;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { expression, openBracketToken, index, closeBracketToken };
    }
}
