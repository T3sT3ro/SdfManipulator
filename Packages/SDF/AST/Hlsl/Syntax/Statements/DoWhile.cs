using System.Collections.Generic;
using AST.Hlsl.Syntax.Expressions;

namespace AST.Hlsl.Syntax.Statements {
    public record DoWhile : Statement {
        public DoKeyword       doKeyword       { get; set; }
        public OpenParenToken  openParenToken  { get; set; }
        public Expression      test            { get; set; }
        public CloseParenToken closeParenToken { get; set; }
        public Statement       body            { get; set; }
        public WhileKeyword    whileKeyword    { get; set; }

        public override IReadOnlyList<HlslSyntax>        ChildNodes          => new HlslSyntax[] { test, body };

        public override IReadOnlyList<HlslSyntaxOrToken> ChildNodesAndTokens => new HlslSyntaxOrToken[]
            { doKeyword, openParenToken, test, closeParenToken, body, whileKeyword };
    }
}
