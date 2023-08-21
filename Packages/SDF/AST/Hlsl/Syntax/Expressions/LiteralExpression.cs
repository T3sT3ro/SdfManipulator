using System.Collections.Generic;

namespace AST.Hlsl.Syntax.Expressions {
    public record LiteralExpression<T> : Expression where T : Literal {
        public T literal { get; init; }

        public override IReadOnlyList<IHlslSyntaxOrToken> ChildNodesAndTokens => new[] { literal };
    }
}
