using System.Collections.Generic;

namespace AST.Hlsl.Syntax.Expressions.Operators {
    // a, b
    public record Comma : Expression {
        public Expression left  { get; set; }
        public CommaToken comma { get; set; } = new();
        public Expression right { get; set; }

        public override IReadOnlyList<IHlslSyntaxOrToken> ChildNodesAndTokens => new IHlslSyntaxOrToken[]
            { left, comma, right };
    }
}
