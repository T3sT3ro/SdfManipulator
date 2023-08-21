using System.Collections.Generic;
using System.Linq;

namespace AST.Hlsl.Syntax.Expressions.Operators {
    // (float)1
    // (float[1])1.0f
    public record Cast : Expression {
        public OpenParenToken                    openParenToken      { get; set; } = new();
        public Type                              type                { get; set; }
        public IReadOnlyList<ArrayRankSpecifier> arrayRankSpecifiers { get; set; }
        public CloseParenToken                   closeParenToken     { get; set; } = new();
        public Expression                        expression          { get; set; }

        public override IReadOnlyList<IHlslSyntaxOrToken> ChildNodesAndTokens =>
            new IHlslSyntaxOrToken[] { openParenToken, type }
                .Concat(arrayRankSpecifiers)
                .Concat(new IHlslSyntaxOrToken[]
                    { closeParenToken, expression })
                .ToList();
    }
}
