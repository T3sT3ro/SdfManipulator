#nullable enable
using System.Collections.Generic;
using System.Linq;
using AST.Hlsl.Syntax.Expressions;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Statements {
    // if (test) then
    // if (test) then else elseStatement
    public record If : Statement {
        public IfKeyword       ifKeyword  { get; set; } = new();
        public OpenParenToken  openParen  { get; set; } = new();
        public Expression      test       { get; set; }
        public CloseParenToken closeParen { get; set; } = new();
        public Statement       then       { get; set; }
        public ElseClause?     elseClause { get; set; }

        public override IReadOnlyList<IHlslSyntaxOrToken> ChildNodesAndTokens => new IHlslSyntaxOrToken[]
            { ifKeyword, openParen, test, closeParen, then }.AppendNotNull(elseClause).ToList();

        public record ElseClause : HlslSyntax {
            public ElseKeyword elseKeyword { get; set; } = new();
            public Statement   statement   { get; set; }

            public override IReadOnlyList<IHlslSyntaxOrToken> ChildNodesAndTokens => new IHlslSyntaxOrToken[]
                { elseKeyword, statement };
        }
    }
}
