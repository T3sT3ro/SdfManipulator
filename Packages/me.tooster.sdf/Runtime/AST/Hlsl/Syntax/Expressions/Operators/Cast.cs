using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators {
    // (float)1
    // (float[1])1.0f
    public partial record Cast : Expression {
        private readonly OpenParenToken              /*_*/openParenToken;
        private readonly Type                        /*_*/type;
        private readonly SyntaxList<Hlsl, ArrayRank> /*_*/arrayRankSpecifiers;
        private readonly CloseParenToken             /*_*/closeParenToken;
        private readonly Expression                  /*_*/expression;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { openParenToken, type, arrayRankSpecifiers, closeParenToken, expression };
    }
}
