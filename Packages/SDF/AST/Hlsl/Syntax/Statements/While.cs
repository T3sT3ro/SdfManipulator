using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Statements {
    public partial record While : Statement {
        private readonly WhileKeyword    _whileKeyword;
        private readonly OpenParenToken  _openParen;
        private readonly Expression      _test;
        private readonly CloseParenToken _closeParen;
        private readonly Statement       _body;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { whileKeyword, openParen, test, closeParen, body };
    }
}
