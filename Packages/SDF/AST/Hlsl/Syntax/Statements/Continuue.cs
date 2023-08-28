using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Statements {
    public record Continuue : Statement {
        public ContinueKeyword continueKeyword { get; init; } = new();
        public SemiToken       semiToken       { get; init; } = new();

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new Token<Hlsl>[]
            { continueKeyword, semiToken };
    }
}
