using System.Collections.Generic;
using AST.Hlsl.Syntax.Expressions;

namespace AST.Hlsl.Syntax.Statements {
    public record While : Statement {
        public          WhileKeyword                     whileKeyword        { get; set; }
        public          OpenParenToken                   openParen           { get; set; }
        public          Expression                       test                { get; set; }
        public          CloseParenToken                  closeParen          { get; set; }
        public          Block                            body                { get; set; }
        
        public override IReadOnlyList<HlslSyntax>        ChildNodes          => new HlslSyntax[]{test, body };

        public override IReadOnlyList<HlslSyntaxOrToken> ChildNodesAndTokens =>
            new HlslSyntaxOrToken[] { whileKeyword, openParen, test, closeParen, body };
    }
}
