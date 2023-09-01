using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators {
    public partial record Unary : Expression {
        private readonly Token<Hlsl> /*_*/operatorToken;
        private readonly Expression  /*_*/expression;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { operatorToken, expression };
    }
}
