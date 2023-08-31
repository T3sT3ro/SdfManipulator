using System.Collections.Generic;
using AST.Syntax;

namespace AST.Shaderlab.Syntax {
    public record LiteralExpression<T> : Syntax<Shaderlab> where T : Literal {
        public Literal literal { get; init; }

        public override IReadOnlyList<SyntaxOrToken<Shaderlab>> ChildNodesAndTokens => new[]
            { literal };
    }
}
