using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators {
    public partial record Ternary : Expression {
        private readonly Expression    /*_*/condition;
        private readonly QuestionToken /*_*/questionToken;
        private readonly Expression    /*_*/whenTrue;
        private readonly ColonToken    /*_*/colonToken;
        private readonly Expression    /*_*/whenFalse;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { condition, questionToken, whenTrue, colonToken, whenFalse };
    }
}
