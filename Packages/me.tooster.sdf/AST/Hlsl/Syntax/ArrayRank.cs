using System.Collections.Generic;
using AST.Hlsl.Syntax.Expressions;
using AST.Syntax;

namespace AST.Hlsl.Syntax {
    public partial record ArrayRank : Syntax<Hlsl> {
        private readonly OpenBracketToken              _openBracketToken;
        private readonly LiteralExpression<IntLiteral> _dimension;
        private readonly CloseBracketToken             _closeBracketToken;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { openBracketToken, dimension, closeBracketToken };
    }
}
