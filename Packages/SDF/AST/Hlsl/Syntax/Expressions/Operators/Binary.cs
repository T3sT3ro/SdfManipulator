using System.Collections.Generic;

namespace AST.Hlsl.Syntax.Expressions.Operators {
    public record Binary : Expression {
        public Expression left          { get; set; }
        public HlslToken  operatorToken { get; set; }
        public Expression right         { get; set; }

        public override IReadOnlyList<IHlslSyntaxOrToken> ChildNodesAndTokens =>
            new IHlslSyntaxOrToken[] { left, operatorToken, right };
    }
}
