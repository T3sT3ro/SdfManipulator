using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Expressions {
    public record Parenthesized : Expression {
        public OpenParenToken  openParen  { get; init; } = new();
        public Expression      expression { get; init; }
        public CloseParenToken closeParen { get; init; } = new();

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { openParen, expression, closeParen };
    }
}
