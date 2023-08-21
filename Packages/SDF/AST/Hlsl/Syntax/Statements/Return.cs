#nullable enable
using System.Collections.Generic;
using System.Linq;
using AST.Hlsl.Syntax.Expressions;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Statements {
    public record Return : Statement {
        public ReturnKeyword returnKeyword { get; set; } = new();
        public Expression?   expression    { get; init; }

        public override IReadOnlyList<IHlslSyntaxOrToken> ChildNodesAndTokens =>
            new IHlslSyntaxOrToken[] { returnKeyword }
                .AppendNotNull(expression)
                .ToList();
    }
}
