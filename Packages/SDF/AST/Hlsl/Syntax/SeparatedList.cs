using System.Collections.Generic;
using System.Linq;
using AST.Syntax;
using Expression = AST.Hlsl.Syntax.Expressions.Expression;

namespace AST.Hlsl.Syntax {
    public record HlslSeparatedSyntaxList : SeparatedSyntaxList<HlslSyntax, HlslToken, HlslSyntaxOrToken> {
        public override IReadOnlyList<HlslSyntax>        ChildNodes          { get; }
        public override IReadOnlyList<HlslSyntaxOrToken> ChildNodesAndTokens { get; }
    }

    public record ArgumentList : HlslSeparatedSyntaxList {
        public HlslToken                 OpenParenToken  { get; set; }
        public IReadOnlyList<Expression> Arguments       { get; set; }
        public HlslToken                 CloseParenToken { get; set; }

        public override IReadOnlyList<HlslSyntax> ChildNodes => Arguments;

        public override IReadOnlyList<HlslSyntaxOrToken> ChildNodesAndTokens =>
            new[] { OpenParenToken }
                .Concat<HlslSyntaxOrToken>(Arguments)
                .Append(CloseParenToken)
                .ToArray();
    }
}
