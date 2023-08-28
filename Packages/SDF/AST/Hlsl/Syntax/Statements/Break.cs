using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Statements {
    public record Break : Statement {
        public BreakKeyword breakKeyword { get; init; } = new();
        public SemiToken    semiToken    { get; init; } = new();

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new Token<Hlsl>[]
            { breakKeyword, semiToken };
    }
}
