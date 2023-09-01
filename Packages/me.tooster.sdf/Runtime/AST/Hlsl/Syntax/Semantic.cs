using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax {
    [AstSyntax]
    public partial record Semantic : Syntax<Hlsl> {
        private readonly ColonToken    _colonToken;
        private readonly SemanticToken _semanticToken;
        
        public static implicit operator Semantic(SemanticToken token) => new() { semanticToken = token };
    }
}
