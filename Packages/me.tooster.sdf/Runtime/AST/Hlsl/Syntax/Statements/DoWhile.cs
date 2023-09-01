using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    // do { body } while (test);
    public partial record DoWhile : Statement {
        private readonly DoKeyword       /*_*/doKeyword;
        private readonly OpenBraceToken  /*_*/openBraceToken;
        private readonly Statement       /*_*/body;
        private readonly CloseBraceToken /*_*/closeBraceToken;
        private readonly WhileKeyword    /*_*/whileKeyword;
        private readonly OpenParenToken  /*_*/openParenToken;
        private readonly Expression      /*_*/test;
        private readonly CloseParenToken /*_*/closeParenToken;
        private readonly SemiToken       /*_*/semicolonToken;

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
