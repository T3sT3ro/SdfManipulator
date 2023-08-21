using System.Collections.Generic;

namespace AST.Hlsl.Syntax.Statements {
    public record Discard : Statement {
        public DiscardKeyword discardKeyword { get; set; } = new();
        public SemiToken      semiToken      { get; set; } = new();

        public override IReadOnlyList<IHlslSyntaxOrToken> ChildNodesAndTokens => new HlslToken[]
            { discardKeyword, semiToken };
    }
}
