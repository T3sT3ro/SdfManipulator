using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators {
    public partial record Binary : Expression {
        private readonly Expression  /*_*/left;
        private readonly Token<Hlsl> /*_*/operatorToken;
        private readonly Expression  /*_*/right;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { left, operatorToken, right };
    }
}
