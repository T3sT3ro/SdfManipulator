using System;
using System.Collections.Generic;

namespace AST.Hlsl.Syntax.Statements {
    public record Continuue : Statement {
        public ContinueKeyword continueKeyword { get; set; }

        public override IReadOnlyList<HlslSyntax>        ChildNodes          => Array.Empty<HlslSyntax>();
        public override IReadOnlyList<HlslSyntaxOrToken> ChildNodesAndTokens => new[] { continueKeyword };
    }
}
