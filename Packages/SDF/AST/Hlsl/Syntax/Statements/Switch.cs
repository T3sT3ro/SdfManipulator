using System.Collections.Generic;
using System.Linq;

namespace AST.Hlsl.Syntax.Statements {
    public partial record Switch : Statement {
        public SwitchKeyword            switchKeyword { get; set; }
        public OpenParenToken           openParen     { get; set; }
        public IdentifierName           selector      { get; set; }
        public CloseParenToken          closeParen    { get; set; }
        public IReadOnlyList<Case>      cases         { get; set; }
        public IReadOnlyList<Statement> @default      { get; set; }

        public override IReadOnlyList<HlslSyntax> ChildNodes =>
            new HlslSyntax[] { selector }
                .Concat(cases)
                .Concat(@default).ToList();

        public override IReadOnlyList<HlslSyntaxOrToken> ChildNodesAndTokens { get; }
    }
}
