using System.Collections.Generic;

namespace AST.Hlsl.Syntax.Expressions.Operators {
    // something[index]
    public record Indexer : Expression {
        public Expression        expression        { get; set; }
        public OpenBracketToken  openBracketToken  { get; set; } = new();
        public Expression        index             { get; set; }
        public CloseBracketToken closeBracketToken { get; set; } = new();

        public override IReadOnlyList<IHlslSyntaxOrToken> ChildNodesAndTokens => new IHlslSyntaxOrToken[]
            { expression, openBracketToken, index, closeBracketToken };
    }
}
