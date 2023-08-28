using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Expressions.Operators {
    // something[index]
    public record Indexer : Expression {
        public Expression        expression        { get; init; }
        public OpenBracketToken  openBracketToken  { get; init; } = new();
        public Expression        index             { get; init; }
        public CloseBracketToken closeBracketToken { get; init; } = new();

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { expression, openBracketToken, index, closeBracketToken };
    }
}
