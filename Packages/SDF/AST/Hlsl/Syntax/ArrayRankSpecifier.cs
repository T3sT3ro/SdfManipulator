using System.Collections.Generic;
using AST.Hlsl.Syntax.Expressions;

namespace AST.Hlsl.Syntax {
    public record ArrayRankSpecifier : HlslSyntax {
        public OpenBracketToken              openBracketToken  { get; init; } = new();
        public LiteralExpression<IntLiteral> dimension         { get; init; }
        public CloseBracketToken             closeBracketToken { get; init; } = new();

        public override IReadOnlyList<IHlslSyntaxOrToken> ChildNodesAndTokens => new IHlslSyntaxOrToken[]
            { openBracketToken, dimension, closeBracketToken };
    }
}
