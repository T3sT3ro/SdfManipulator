using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Expressions {
    public partial record Parenthesized : Expression {
        private readonly OpenParenToken  _openParen;
        private readonly Expression      _expression;
        private readonly CloseParenToken _closeParen;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { openParen, expression, closeParen };
    }
}
