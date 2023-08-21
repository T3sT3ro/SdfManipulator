using System.Collections.Generic;
using AST.Hlsl.Syntax.Expressions;

namespace AST.Hlsl.Syntax.Statements {
    public record While : Statement {
        public WhileKeyword    whileKeyword { get; set; } = new();
        public OpenParenToken  openParen    { get; set; } = new();
        public Expression      test         { get; set; }
        public CloseParenToken closeParen   { get; set; } = new();
        public Statement       body         { get; set; }

        public override IReadOnlyList<IHlslSyntaxOrToken> ChildNodesAndTokens => new IHlslSyntaxOrToken[]
            { whileKeyword, openParen, test, closeParen, body };
    }
}
