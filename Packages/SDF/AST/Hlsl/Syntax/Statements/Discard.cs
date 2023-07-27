#nullable enable
using System;
using System.Collections.Generic;

namespace AST.Hlsl.Syntax.Statements {
    public record Discard : Statement {
        public DiscardKeyword discardKeyword { get; set; }

        public override IReadOnlyList<HlslSyntax>        ChildNodes          => Array.Empty<HlslSyntax>();
        public override IReadOnlyList<HlslSyntaxOrToken> ChildNodesAndTokens => new[] { discardKeyword };
    }
}
