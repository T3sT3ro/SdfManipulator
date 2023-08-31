using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax {
    public partial record Semantic : Syntax<Hlsl> {
        private readonly ColonToken    _colonToken;
        private readonly SemanticToken _semanticToken;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { colonToken, semanticToken };

        public static implicit operator Semantic(SemanticToken token) => new() { semanticToken = token };
    }
}
