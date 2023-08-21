using System.Collections.Generic;

namespace AST.Hlsl.Syntax.Statements {
    public record Break : Statement {
        public BreakKeyword breakKeyword { get; set; } = new();
        public SemiToken    semiToken    { get; set; } = new();

        public override IReadOnlyList<IHlslSyntaxOrToken> ChildNodesAndTokens => new HlslToken[]
            { breakKeyword, semiToken };
    }
}
