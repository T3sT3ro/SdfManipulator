using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Expressions {
    public partial record LiteralExpression<T> : Expression where T : Literal {
        private readonly T /*_*/literal;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new[]
            { literal };
    }
}
