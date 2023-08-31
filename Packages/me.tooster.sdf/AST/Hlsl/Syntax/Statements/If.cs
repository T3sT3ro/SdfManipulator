#nullable enable
using System.Collections.Generic;
using System.Linq;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Statements {
    // if (test) then
    // if (test) then else elseStatement
    public partial record If : Statement {
        private readonly IfKeyword       _ifKeyword;
        private readonly OpenParenToken  _openParen;
        private readonly Expression      _test;
        private readonly CloseParenToken _closeParen;
        private readonly Statement       _then;
        private readonly ElseClause?     _elseClause;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>?[]
            { ifKeyword, openParen, test, closeParen, then, elseClause }.FilterNotNull().ToList();
    }
}
