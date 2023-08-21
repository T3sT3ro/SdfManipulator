using System.Collections.Generic;

namespace AST.Hlsl.Syntax.Expressions {
    // {EXPR, EPXR, ...}
    public record StructInitializer : Expression {
        public BracketInitializerList<Expression> components { get; init; }

        public override IReadOnlyList<IHlslSyntaxOrToken> ChildNodesAndTokens => components;
    }
}
