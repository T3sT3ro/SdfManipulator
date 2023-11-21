using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax {
    [SyntaxNode] public partial record Semantic : Syntax<hlsl> {
        public ColonToken    colonToken    { get; init; } = new();
        public SemanticToken semanticToken { get; init; }

        public static implicit operator Semantic(SemanticToken token) => new() { semanticToken = token };
    }
}
