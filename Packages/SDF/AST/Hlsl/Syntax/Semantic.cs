using System.Collections.Generic;

namespace AST.Hlsl.Syntax {
    public record Semantic : HlslSyntax {
        public ColonToken    colonToken    { get; set; } = new ColonToken();
        public SemanticToken semanticToken { get; init; }

        public override IReadOnlyList<IHlslSyntaxOrToken> ChildNodesAndTokens => new IHlslSyntaxOrToken[]
            { colonToken, semanticToken };
        
        public static implicit operator Semantic(SemanticToken token) => new Semantic { semanticToken = token };
    }
}
