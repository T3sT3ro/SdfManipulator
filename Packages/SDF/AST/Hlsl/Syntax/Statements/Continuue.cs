using System.Collections.Generic;

namespace AST.Hlsl.Syntax.Statements {
    public record Continuue : Statement {
        public ContinueKeyword continueKeyword { get; set; } = new();
        public SemiToken       semiToken       { get; set; } = new();

        public override IReadOnlyList<IHlslSyntaxOrToken> ChildNodesAndTokens => new HlslToken[] 
            { continueKeyword, semiToken };
    }
}
