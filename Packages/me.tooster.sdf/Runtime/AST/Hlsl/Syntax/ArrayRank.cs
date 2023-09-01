using System.Collections.Generic;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax {
    public partial record ArrayRank : Syntax<Hlsl> {
        private readonly OpenBracketToken              /*_*/openBracketToken;
        private readonly LiteralExpression<IntLiteral> /*_*/dimension;
        private readonly CloseBracketToken             /*_*/closeBracketToken;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { openBracketToken, dimension, closeBracketToken };
    }
}
