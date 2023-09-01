#nullable enable
using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    // expression;
    // ; <- empty expression statement
    public partial record ExpressionStatement : Statement {
        private readonly Expression? /*_*/expression;
        private readonly SemiToken   /*_*/semiToken;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens =>
            expression != null
                ? new SyntaxOrToken<Hlsl>[] { expression, semiToken }
                : new SyntaxOrToken<Hlsl>[] { semiToken };
    }
}
