using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Expressions {
    public record LiteralExpression<T> : Expression where T : Literal {
        public T literal { get; init; }

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new[]
            { literal };
    }
}
