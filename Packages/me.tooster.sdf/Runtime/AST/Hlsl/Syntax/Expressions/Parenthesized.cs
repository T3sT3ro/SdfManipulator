using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Expressions {
    public partial record Parenthesized : Expression {
        private readonly OpenParenToken  /*_*/openParen;
        private readonly Expression      /*_*/expression;
        private readonly CloseParenToken /*_*/closeParen;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { openParen, expression, closeParen };
    }
}
