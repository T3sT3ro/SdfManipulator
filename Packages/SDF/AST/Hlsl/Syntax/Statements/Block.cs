using System.Collections.Generic;
using System.Linq;

namespace AST.Hlsl.Syntax.Statements {
    public record Block : Statement {
        public OpenBraceToken           openBraceToken  { get; set; } = new();
        public IReadOnlyList<Statement> statements      { get; set; }
        public CloseBraceToken          closeBraceToken { get; set; } = new();

        public override IReadOnlyList<IHlslSyntaxOrToken> ChildNodesAndTokens =>
            statements
                .Prepend<IHlslSyntaxOrToken>(openBraceToken)
                .Append(closeBraceToken)
                .ToList();
    }
}
