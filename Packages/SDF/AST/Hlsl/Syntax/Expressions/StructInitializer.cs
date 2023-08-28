using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Expressions {
    // {EXPR, EPXR, ...}
    public record StructInitializer : Expression {
        public BracedList<Expression> components { get; init; } = new();

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { components };
    }
}
