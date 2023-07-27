using System;
using System.Collections.Generic;

namespace AST.Hlsl.Syntax {
    public record IdentifierName : HlslSyntax {
        public IdentifierToken idToken { get; internal set; }

        public override IReadOnlyList<HlslSyntax>        ChildNodes          => Array.Empty<HlslSyntax>();
        public override IReadOnlyList<HlslSyntaxOrToken> ChildNodesAndTokens => new HlslSyntaxOrToken[] { idToken };
    }
}
