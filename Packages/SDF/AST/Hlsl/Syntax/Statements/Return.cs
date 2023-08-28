#nullable enable
using System.Collections.Generic;
using System.Linq;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Statements {
    public record Return : Statement {
        public ReturnKeyword returnKeyword { get; init; } = new();
        public Expression?   expression    { get; init; }

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens =>
            new SyntaxOrToken<Hlsl>[] { returnKeyword }
                .AppendNotNull(expression)
                .ToList();
    }
}
