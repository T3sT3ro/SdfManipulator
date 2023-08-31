#nullable enable
using System.Collections.Generic;
using System.Linq;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Statements {
    public partial record Switch : Statement {
        private readonly SwitchKeyword          _switchKeyword;
        private readonly OpenParenToken         _openParen;
        private readonly Identifier             _selector;
        private readonly CloseParenToken        _closeParen;
        private readonly SyntaxList<Hlsl, Case> _cases;
        public DefaultCase?           @default      { get; init; }

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>?[]
            { switchKeyword, openParen, selector, closeParen, cases, @default }.FilterNotNull().ToList();
    }
}
