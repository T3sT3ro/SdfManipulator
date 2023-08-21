using System.Collections.Generic;

namespace AST.Hlsl.Syntax.Expressions.Operators {
    public record Unary : Expression {
        public HlslToken  operatorToken { get; set; }
        public Expression expression    { get; set; }

        public override IReadOnlyList<IHlslSyntaxOrToken> ChildNodesAndTokens =>
            new IHlslSyntaxOrToken[] { operatorToken, expression };
    }
}
