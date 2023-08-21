using System.Collections.Generic;
using AST.Hlsl.Syntax.Expressions;

namespace AST.Hlsl.Syntax {
    public record ArrayRankSpecifier : HlslSyntax {
        public OpenBracketToken              openBracketToken  { get; internal set; }
        public LiteralExpression<IntLiteral> dimension         { get; internal set; }
        public CloseBracketToken             closeBracketToken { get; internal set; }

        public override IReadOnlyList<IHlslSyntaxOrToken> ChildNodesAndTokens => new IHlslSyntaxOrToken[]
            { openBracketToken, dimension, closeBracketToken };
    }
}
