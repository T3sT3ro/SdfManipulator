using System.Collections.Generic;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax {
    [Syntax] public partial record ArrayRank : Syntax<Hlsl> {
        [Init] private readonly OpenBracketToken              _openBracketToken;
        private readonly        LiteralExpression<IntLiteral> _dimension;
        [Init] private readonly CloseBracketToken             _closeBracketToken;
    }
}
