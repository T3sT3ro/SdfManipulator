using System.Collections.Generic;

namespace AST.Hlsl.Syntax.Expressions.Operators {
    public record Unary : Expression {
        public HlslToken  operatorToken { get; set; }
        public Expression expression    { get; set; }

        public override IReadOnlyList<HlslSyntax>        ChildNodes          => new[] { expression };
        public override IReadOnlyList<HlslSyntaxOrToken> ChildNodesAndTokens => new HlslSyntaxOrToken[] { operatorToken, expression };
    }
}
