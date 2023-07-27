using System;
using System.Collections.Generic;
using System.Linq;
using AST.Syntax;

namespace AST.Hlsl.Syntax {
    public record Semantic : HlslSyntax {
        public HlslToken SemanticKind { get; internal set; }
        public uint?     index        { get; internal set; }

        public override IReadOnlyList<HlslSyntax>        ChildNodes          => Array.Empty<HlslSyntax>();
        public override IReadOnlyList<HlslSyntaxOrToken> ChildNodesAndTokens => new HlslSyntaxOrToken[] { SemanticKind };
    }
}