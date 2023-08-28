using System.Collections.Generic;
using AST.Hlsl.Syntax.Expressions;
using AST.Syntax;

namespace AST.Hlsl.Syntax {
    public record ArrayRank : Syntax<Hlsl> {
        public OpenBracketToken              openBracketToken  { get; init; }
        public LiteralExpression<IntLiteral> dimension         { get; init; }
        public CloseBracketToken             closeBracketToken { get; init; } = new();

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { openBracketToken, dimension, closeBracketToken };
    }
}
