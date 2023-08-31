using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Statements {
    // do { body } while (test);
    public partial record DoWhile : Statement {
        private readonly DoKeyword       _doKeyword;
        private readonly OpenBraceToken  _openBraceToken;
        private readonly Statement       _body;
        private readonly CloseBraceToken _closeBraceToken;
        private readonly WhileKeyword    _whileKeyword;
        private readonly OpenParenToken  _openParenToken;
        private readonly Expression      _test;
        private readonly CloseParenToken _closeParenToken;
        private readonly SemiToken       _semicolonToken;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
        {
            doKeyword,
            openBraceToken, body, closeBraceToken,
            whileKeyword,
            openParenToken, test, closeParenToken,
            semicolonToken,
        };
    }
}
