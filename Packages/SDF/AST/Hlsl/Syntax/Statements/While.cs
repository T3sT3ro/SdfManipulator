using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Statements {
    public record While : Statement {
        public WhileKeyword    whileKeyword { get; init; } = new();
        public OpenParenToken  openParen    { get; init; } = new();
        public Expression      test         { get; init; }
        public CloseParenToken closeParen   { get; init; } = new();
        public Statement       body         { get; init; }

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { whileKeyword, openParen, test, closeParen, body };
    }
}
