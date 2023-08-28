#nullable enable
using System.Collections.Generic;
using System.Linq;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Statements {
    public partial record Switch : Statement {
        public SwitchKeyword          switchKeyword { get; init; } = new();
        public OpenParenToken         openParen     { get; init; } = new();
        public Identifier             selector      { get; init; }
        public CloseParenToken        closeParen    { get; init; } = new();
        public SyntaxList<Hlsl, Case> cases         { get; init; } = SyntaxList<Hlsl, Case>.Empty;
        public DefaultCase?           @default      { get; init; }

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>?[]
            { switchKeyword, openParen, selector, closeParen, cases, @default }.FilterNotNull().ToList();
    }
}
