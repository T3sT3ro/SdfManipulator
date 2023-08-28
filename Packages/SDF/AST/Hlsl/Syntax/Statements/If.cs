#nullable enable
using System.Collections.Generic;
using System.Linq;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Statements {
    // if (test) then
    // if (test) then else elseStatement
    public partial record If : Statement {
        public IfKeyword       ifKeyword  { get; init; } = new();
        public OpenParenToken  openParen  { get; init; } = new();
        public Expression      test       { get; init; }
        public CloseParenToken closeParen { get; init; } = new();
        public Statement       then       { get; init; }
        public ElseClause?     elseClause { get; init; }

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>?[]
            { ifKeyword, openParen, test, closeParen, then, elseClause }.FilterNotNull().ToList();
    }
}
