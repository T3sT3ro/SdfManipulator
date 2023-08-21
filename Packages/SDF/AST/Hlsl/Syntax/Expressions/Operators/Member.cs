using System.Collections.Generic;

namespace AST.Hlsl.Syntax.Expressions.Operators {
    // a.b.c
    public record Member : Expression {
        public Expression expression { get; init; }
        public DotToken   dotToken   { get; set; } = new();
        public Identifier member     { get; init; }

        public override IReadOnlyList<IHlslSyntaxOrToken> ChildNodesAndTokens =>
            new IHlslSyntaxOrToken[] { expression, dotToken, member };
    }
}
