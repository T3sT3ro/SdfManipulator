using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators {
    // a.b.c
    public partial record Member : Expression {
        private readonly Expression                  /*_*/expression;
        private readonly DotToken                    /*_*/dotToken;
        private readonly Identifier /*_*/member;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { expression, dotToken, member };
    }
}
