using System.Collections.Generic;

namespace AST.Hlsl.Syntax.Expressions {
    public record Parenthesized : Expression {
        public OpenBraceToken openBraceToken { get; private set; }
        public Expression expression { get; private set; }
        public CloseBraceToken openParenToken { get; private set; }

        public static   Parenthesized                    From(Expression expr) => new Parenthesized { expression = expr };
        public override IReadOnlyList<HlslSyntax>        ChildNodes            => new HlslSyntax[] { expression };
        public override IReadOnlyList<HlslSyntaxOrToken> ChildNodesAndTokens   => new HlslSyntaxOrToken[] { openBraceToken, expression, openParenToken };
    }
}
