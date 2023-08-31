using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Expressions.Operators {
    // a, b
    public partial record Comma : Expression {
        private readonly Expression _left;
        private readonly CommaToken _comma;
        private readonly Expression _right;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { left, comma, right };
    }
}
