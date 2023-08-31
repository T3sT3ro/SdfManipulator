using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Expressions.Operators {
    // a.b.c
    public partial record Member : Expression {
        private readonly Expression _expression;
        private readonly DotToken   _dotToken;
        private readonly Identifier _member;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { expression, dotToken, member };
    }
}
