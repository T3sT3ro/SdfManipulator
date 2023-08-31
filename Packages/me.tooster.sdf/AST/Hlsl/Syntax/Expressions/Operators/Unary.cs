using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Expressions.Operators {
    public partial record Unary : Expression {
        private readonly Token<Hlsl> _operatorToken;
        private readonly Expression  _expression;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { operatorToken, expression };
    }
}
