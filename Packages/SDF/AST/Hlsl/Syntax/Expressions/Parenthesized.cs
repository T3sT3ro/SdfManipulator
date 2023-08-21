using System.Collections.Generic;

namespace AST.Hlsl.Syntax.Expressions {
    public record Parenthesized : Expression {
        public OpenParenToken  openParen  { get; set; } = new();
        public Expression      expression { get; init; }
        public CloseParenToken closeParen { get; set; } = new();

        public override IReadOnlyList<IHlslSyntaxOrToken> ChildNodesAndTokens => new IHlslSyntaxOrToken[]
            { openParen, expression, closeParen };
    }
}
