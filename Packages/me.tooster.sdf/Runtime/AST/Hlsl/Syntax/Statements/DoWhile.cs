using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    // do { body } while (test);
    [SyntaxNode] public partial record DoWhile : Statement {
        public DoKeyword       doKeyword       { get; init; } = new();
        public OpenBraceToken  openBraceToken  { get; init; } = new();
        public Statement       body            { get; init; }
        public CloseBraceToken closeBraceToken { get; init; } = new();
        public WhileKeyword    whileKeyword    { get; init; } = new();
        public OpenParenToken  openParenToken  { get; init; } = new();
        public Expression      test            { get; init; }
        public CloseParenToken closeParenToken { get; init; } = new();
        public SemicolonToken  semicolonToken  { get; init; } = new();
    }
}
