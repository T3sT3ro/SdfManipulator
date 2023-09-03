#nullable enable
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
   [Syntax] public partial record Switch : Statement {
        private readonly SwitchKeyword                                  _switchKeyword;
        private readonly OpenParenToken                                 _openParen;
        private readonly Identifier                    _selector;
        private readonly CloseParenToken                                _closeParen;
        private readonly SyntaxList<Hlsl, Switch.Case> _cases;
        public           Switch.DefaultCase?           @default { get; init; }

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>?[]
            { switchKeyword, openParen, selector, closeParen, cases, @default }.FilterNotNull().ToList();
    }
}
