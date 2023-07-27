using System.Collections.Generic;

namespace AST.Hlsl.Syntax.Expressions.Operators {
    public record Indexer : Expression {
        public Expression       expression       { get; set; }
        public OpenBracketToken openBracketToken { get; set; }
        public Expression       index            { get; set; }
        public CloseBraceToken  closeBraceToken  { get; set; }

        public override IReadOnlyList<HlslSyntax>        ChildNodes          => new[] { expression, index };
        public override IReadOnlyList<HlslSyntaxOrToken> ChildNodesAndTokens => new HlslSyntaxOrToken[] { expression, openBracketToken, index, closeBraceToken };
    }
}
