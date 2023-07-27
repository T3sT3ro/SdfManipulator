using System.Collections.Generic;
using System.Linq;
using AST.Syntax;
using Expression = AST.Hlsl.Syntax.Expressions.Expression;

namespace AST.Hlsl.Syntax {
    public record ArgumentList : SeparatedSyntaxList<HlslSyntax, HlslToken, HlslSyntaxOrToken> {
        public HlslToken                 OpenParenToken  { get; set; }
        public IReadOnlyList<Expression> Arguments       { get; set; }
        public HlslToken                 CloseParenToken { get; set; }

        public ArgumentList(IEnumerable<HlslSyntaxOrToken> list) : base(list) { }

        public override IReadOnlyList<HlslSyntax> ChildNodes => Arguments;

        public override IReadOnlyList<HlslSyntaxOrToken> ChildNodesAndTokens => new[] { OpenParenToken }
            .Concat<HlslSyntaxOrToken>(Arguments).Concat(new[] { CloseParenToken }).ToArray();
    }
}
