#nullable enable
using System;
using System.Collections.Generic;
using AST.Hlsl.Syntax.Expressions;

namespace AST.Hlsl.Syntax.Statements {
    // expression;
    // ; <- empty expression statement
    public record ExpressionStatement : Statement {
        public Expression? expression { get; init; }
        public SemiToken   semiToken  { get; init; } = new();

        public override IReadOnlyList<IHlslSyntaxOrToken> ChildNodesAndTokens =>
            expression != null
                ? new IHlslSyntaxOrToken[] { expression, semiToken }
                : new IHlslSyntaxOrToken[] { semiToken };
    }
}
