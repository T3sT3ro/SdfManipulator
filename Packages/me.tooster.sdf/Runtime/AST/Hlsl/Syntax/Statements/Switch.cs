#nullable enable
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    public partial record Switch : Statement {
        private readonly SwitchKeyword                                  /*_*/switchKeyword;
        private readonly OpenParenToken                                 /*_*/openParen;
        private readonly Identifier                    /*_*/selector;
        private readonly CloseParenToken                                /*_*/closeParen;
        private readonly SyntaxList<Hlsl, Switch.Case> /*_*/cases;
        public           Switch.DefaultCase?           @default { get; init; }

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>?[]
            { switchKeyword, openParen, selector, closeParen, cases, @default }.FilterNotNull().ToList();
    }
}
