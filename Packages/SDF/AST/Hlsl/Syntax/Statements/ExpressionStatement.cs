#nullable enable
using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Statements {
    // expression;
    // ; <- empty expression statement
    public record ExpressionStatement : Statement {
        public Expression? expression { get; init; }
        public SemiToken   semiToken  { get; init; } = new();

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens =>
            expression != null
                ? new SyntaxOrToken<Hlsl>[] { expression, semiToken }
                : new SyntaxOrToken<Hlsl>[] { semiToken };
    }
}
