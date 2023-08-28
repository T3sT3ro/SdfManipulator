using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Statements {
    // = y
    // = {{a}, {b}}}
    public abstract record Initializer : Syntax<Hlsl> {
        public EqualsToken equalsToken { get; init; } = new();
        public Expression  value       { get; init; }

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { equalsToken, value };
    }
}
