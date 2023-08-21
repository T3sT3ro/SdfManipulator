using System.Collections.Generic;

namespace AST.Shaderlab.Syntax {
    public record LiteralExpression<T> : ShaderlabSyntax where T : Literal {
        public Literal literal { get; set; }

        public override IReadOnlyList<IShaderlabSyntaxOrToken> ChildNodesAndTokens => new[] { literal };
    }
}
