using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Expressions.Operators {
    public partial record Ternary : Expression {
        private readonly Expression    _condition;
        private readonly QuestionToken _questionToken;
        private readonly Expression    _whenTrue;
        private readonly ColonToken    _colonToken;
        private readonly Expression    _whenFalse;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { condition, questionToken, whenTrue, colonToken, whenFalse };
    }
}
