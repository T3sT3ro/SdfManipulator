#nullable enable
using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Statements {
    // expression;
    // ; <- empty expression statement
    public partial record ExpressionStatement : Statement {
        private readonly Expression? _expression;
        private readonly SemiToken   _semiToken;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens =>
            expression != null
                ? new SyntaxOrToken<Hlsl>[] { expression, semiToken }
                : new SyntaxOrToken<Hlsl>[] { semiToken };
    }
}
