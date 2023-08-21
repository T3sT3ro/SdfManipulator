#nullable enable
using System.Collections.Generic;
using System.Linq;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Statements {
    public record Switch : Statement {
        public SwitchKeyword       switchKeyword { get; set; } = new();
        public OpenParenToken      openParen     { get; set; } = new();
        public Identifier      selector      { get; set; }
        public CloseParenToken     closeParen    { get; set; } = new();
        public IReadOnlyList<Case> cases         { get; set; }
        public DefaultCase?        @default      { get; set; }

        public override IReadOnlyList<IHlslSyntaxOrToken> ChildNodesAndTokens => new IHlslSyntaxOrToken[]
                { switchKeyword, openParen, selector, closeParen }
            .Concat(cases)
            .AppendNotNull(@default)
            .ToList();


        public record Case : HlslSyntax {
            public CaseKeyword              caseKeyword { get; set; } = new();
            public IntLiteral               label       { get; set; }
            public ColonToken               colonToken  { get; set; } = new();
            public IReadOnlyList<Statement> body        { get; set; }

            public override IReadOnlyList<IHlslSyntaxOrToken> ChildNodesAndTokens =>
                new IHlslSyntaxOrToken[] { caseKeyword, label, colonToken }
                    .Concat(body)
                    .ToList();
        }

        public record DefaultCase : HlslSyntax {
            public DefaultKeyword           defaultKeyword { get; set; } = new();
            public ColonToken               colonToken     { get; set; } = new();
            public IReadOnlyList<Statement> body           { get; set; }

            public override IReadOnlyList<IHlslSyntaxOrToken> ChildNodesAndTokens =>
                new IHlslSyntaxOrToken[] { defaultKeyword, colonToken }
                    .Concat(body)
                    .ToList();
        }
    }
}
