#nullable enable
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    // if (test) then
    // if (test) then else elseStatement
    public partial record If : Statement {
        private readonly IfKeyword                       /*_*/ifKeyword;
        private readonly OpenParenToken                  /*_*/openParen;
        private readonly Expression                      /*_*/test;
        private readonly CloseParenToken                 /*_*/closeParen;
        private readonly Statement                       /*_*/then;
        private readonly If.ElseClause? /*_*/elseClause;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>?[]
            { ifKeyword, openParen, test, closeParen, then, elseClause }.FilterNotNull().ToList();
    }
}
