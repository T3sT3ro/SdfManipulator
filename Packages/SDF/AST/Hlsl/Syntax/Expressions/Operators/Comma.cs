using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Expressions.Operators {
    // a, b
    public record Comma : Expression {
        public Expression left  { get; init; }
        public CommaToken comma { get; init; } = new();
        public Expression right { get; init; }

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { left, comma, right };
    }
}
