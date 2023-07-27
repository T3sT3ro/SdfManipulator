using System.Collections.Generic;
using AST.Hlsl.Syntax.Expressions;

namespace AST.Hlsl.Syntax.Statements {
    public record ExpressionStatement : Statement {
        public Expression expression { get; set; }
        public SemiToken  semiToken  { get; }

        public override IReadOnlyList<HlslSyntax>        ChildNodes          => new[] { expression };
        public override IReadOnlyList<HlslSyntaxOrToken> ChildNodesAndTokens => new HlslSyntaxOrToken[] { expression, semiToken };
    }
}
