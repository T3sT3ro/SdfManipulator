using System.Collections.Generic;
using AST.Hlsl.Syntax.Expressions;

namespace AST.Hlsl.Syntax.Statements {
    // expression;
    public record ExpressionStatement : Statement {
        public Expression expression { get; init; }
        public SemiToken  semiToken  { get; set;  } = new();

        public override IReadOnlyList<IHlslSyntaxOrToken> ChildNodesAndTokens => new IHlslSyntaxOrToken[]
            { expression, semiToken };
    }
}
