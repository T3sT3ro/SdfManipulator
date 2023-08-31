using System.Collections.Generic;
using System.Linq;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Expressions.Operators {
    // (float)1
    // (float[1])1.0f
    public partial record Cast : Expression {
        private readonly OpenParenToken              _openParenToken;
        private readonly Type                        _type;
        private readonly SyntaxList<Hlsl, ArrayRank> _arrayRankSpecifiers;
        private readonly CloseParenToken             _closeParenToken;
        private readonly Expression                  _expression;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { openParenToken, type, arrayRankSpecifiers, closeParenToken, expression };
    }
}
