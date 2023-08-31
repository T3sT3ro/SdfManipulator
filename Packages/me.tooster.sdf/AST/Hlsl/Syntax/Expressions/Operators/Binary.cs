using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Expressions.Operators {
    public partial record Binary : Expression {
        private readonly Expression  _left;
        private readonly Token<Hlsl> _operatorToken;
        private readonly Expression  _right;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { left, operatorToken, right };
    }
}
