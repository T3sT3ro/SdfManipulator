using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax {
    [AstSyntax] public partial record Semantic : Syntax<Hlsl> {
        public ColonToken    colonToken { get; init; } = new();
        public        SemanticToken semanticToken { get; init; }

        public static implicit operator Semantic(SemanticToken token) => new() { semanticToken = token };
    }
}
