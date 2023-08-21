using System.Collections.Generic;
using AST.Hlsl.Syntax.Expressions;

namespace AST.Hlsl.Syntax.Statements {
    // do { body } while (test);
    public record DoWhile : Statement {
        public DoKeyword       doKeyword       { get; set; } = new();
        public OpenBraceToken  openBraceToken  { get; set; } = new();
        public Statement       body            { get; set; }
        public CloseBraceToken closeBraceToken { get; set; } = new();
        public WhileKeyword    whileKeyword    { get; set; } = new();
        public OpenParenToken  openParenToken  { get; set; } = new();
        public Expression      test            { get; set; }
        public CloseParenToken closeParenToken { get; set; } = new();
        public SemiToken       semicolonToken  { get; set; } = new();

        public override IReadOnlyList<IHlslSyntaxOrToken> ChildNodesAndTokens => new IHlslSyntaxOrToken[]
        {
            doKeyword,
            openBraceToken, body, closeBraceToken,
            whileKeyword,
            openParenToken, test, closeParenToken,
            semicolonToken
        };
    }
}
