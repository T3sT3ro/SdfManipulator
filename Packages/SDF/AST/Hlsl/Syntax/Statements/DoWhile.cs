using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Statements {
    // do { body } while (test);
    public record DoWhile : Statement {
        public DoKeyword       doKeyword       { get; init; } = new();
        public OpenBraceToken  openBraceToken  { get; init; } = new();
        public Statement       body            { get; init; }
        public CloseBraceToken closeBraceToken { get; init; } = new();
        public WhileKeyword    whileKeyword    { get; init; } = new();
        public OpenParenToken  openParenToken  { get; init; } = new();
        public Expression      test            { get; init; }
        public CloseParenToken closeParenToken { get; init; } = new();
        public SemiToken       semicolonToken  { get; init; } = new();

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
        {
            doKeyword,
            openBraceToken, body, closeBraceToken,
            whileKeyword,
            openParenToken, test, closeParenToken,
            semicolonToken,
        };
    }
}
