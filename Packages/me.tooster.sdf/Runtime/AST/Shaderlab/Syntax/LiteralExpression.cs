using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax {
    public record LiteralExpression<T> : Syntax<Shaderlab> where T : Literal {
        public Literal _literal { get; init; }

        public override IReadOnlyList<SyntaxOrToken<Shaderlab>> ChildNodesAndTokens => new[]
            { literal };
    }
}
