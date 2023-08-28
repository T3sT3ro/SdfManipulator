using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax {
    public record Semantic : Syntax<Hlsl> {
        public ColonToken    colonToken    { get; init; } = new();
        public SemanticToken semanticToken { get; init; }

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { colonToken, semanticToken };

        public static implicit operator Semantic(SemanticToken token) => new() { semanticToken = token };
    }
}
