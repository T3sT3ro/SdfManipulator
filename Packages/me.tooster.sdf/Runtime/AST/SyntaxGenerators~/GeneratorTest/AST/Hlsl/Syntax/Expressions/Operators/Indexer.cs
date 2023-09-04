using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators {
    // something[index]
    [Syntax] public partial record Indexer : Expression {
        private readonly        Expression        _expression;
        [Init] private readonly OpenBracketToken  _openBracketToken;
        private readonly        Expression        _index;
        [Init] private readonly CloseBracketToken _closeBracketToken;
    }
}
