using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators {
    // (float)1
    // (float[1])1.0f
    [Syntax] public partial record Cast : Expression {
        [Init] private readonly OpenParenToken              _openParenToken;
        private readonly        Type                        _type;
        [Init] private readonly SyntaxList<Hlsl, ArrayRank> _arrayRankSpecifiers;
        [Init] private readonly CloseParenToken             _closeParenToken;
        private readonly        Expression                  _expression;
    }
}
