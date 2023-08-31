using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Expressions.Operators {
    // something[index]
    public partial record Indexer : Expression {
        private readonly Expression        _expression;
        private readonly OpenBracketToken  _openBracketToken;
        private readonly Expression        _index;
        private readonly CloseBracketToken _closeBracketToken;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { expression, openBracketToken, index, closeBracketToken };
    }
}
