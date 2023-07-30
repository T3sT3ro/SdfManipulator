using System.Collections.Generic;
using System.Linq;
using AST.Hlsl.Syntax.Expressions.Literals;

namespace AST.Hlsl.Syntax.Statements {
    public partial record Switch {
        public record Case : HlslSyntax {
            public CaseKeyword               caseKeyword { get; set; }
            public Literal                   label       { get; set; }
            public ColonToken                colonToken  { get; set; }
            public IReadOnlyList<HlslSyntax> body        { get; set; }

            public override IReadOnlyList<HlslSyntax> ChildNodes => body.Prepend(label).ToArray();

            public override IReadOnlyList<HlslSyntaxOrToken> ChildNodesAndTokens =>
                new HlslSyntaxOrToken[] { caseKeyword, label, colonToken }.Concat(body).ToArray();
        }
    }
}
