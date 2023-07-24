using System;
using System.Collections.Generic;

namespace AST.Hlsl.Syntax {
    public record IdentifierName : HlslSyntax {
        public HlslToken.IdentifierToken identifier { get; internal set; }

        public override IList<HlslSyntax>        ChildNodes          => Array.Empty<HlslSyntax>();
        public override IList<HlslSyntaxOrToken> ChildNodesAndTokens => new HlslSyntaxOrToken[] { identifier };
    }
}
