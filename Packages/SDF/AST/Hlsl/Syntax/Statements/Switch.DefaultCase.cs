#nullable enable
using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Statements {
    public partial record Switch {
        public record DefaultCase : Syntax<Hlsl> {
            public DefaultKeyword              defaultKeyword { get; init; } = new();
            public ColonToken                  colonToken     { get; init; } = new();
            public SyntaxList<Hlsl, Statement> body           { get; init; } = SyntaxList<Hlsl, Statement>.Empty;

            public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
                { defaultKeyword, colonToken, body };
        }
    }
}
