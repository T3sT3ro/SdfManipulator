using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    public partial record While : Statement {
        private readonly WhileKeyword    /*_*/whileKeyword;
        private readonly OpenParenToken  /*_*/openParen;
        private readonly Expression      /*_*/test;
        private readonly CloseParenToken /*_*/closeParen;
        private readonly Statement       /*_*/body;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { whileKeyword, openParen, test, closeParen, body };
    }
}
