using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators {
    // a, b
    public partial record Comma : Expression {
        private readonly Expression /*_*/left;
        private readonly CommaToken /*_*/comma;
        private readonly Expression /*_*/right;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { left, comma, right };
    }
}
