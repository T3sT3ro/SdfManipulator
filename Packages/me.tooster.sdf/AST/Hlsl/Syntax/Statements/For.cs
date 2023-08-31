#nullable enable
using System.Collections.Generic;
using System.Linq;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Statements {
    // for (start conditions; conditions; increments) { body }
    // start conditions:
    //    declarations: int i, j = 0;
    //    initializers: i = 0, j = 0;
    public partial record For : Statement {
        private readonly ForKeyword                       _forKeyword;
        private readonly OpenParenToken                   _openParen;
        private readonly Initializer?                     _initializer;
        private readonly SemiToken                        _firstSemiToken;
        private readonly Expression?                      _condition;
        private readonly SemiToken                        _secondSemiToken;
        private readonly SeparatedList<Hlsl, Expression>? _increments;
        private readonly CloseParenToken                  _closeParen;
        private readonly Statement                        _body;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>?[]
            {
                forKeyword, openParen, initializer, firstSemiToken, condition, secondSemiToken, increments, closeParen,
                body,
            }.FilterNotNull()
            .ToList();

        public abstract partial record Initializer : Syntax<Hlsl>;
    }
}
