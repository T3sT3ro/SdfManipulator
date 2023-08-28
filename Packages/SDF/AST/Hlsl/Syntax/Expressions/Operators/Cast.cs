using System.Collections.Generic;
using System.Linq;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Expressions.Operators {
    // (float)1
    // (float[1])1.0f
    public record Cast : Expression {
        public OpenParenToken              openParenToken      { get; init; } = new();
        public Type                        type                { get; init; }
        public SyntaxList<Hlsl, ArrayRank> arrayRankSpecifiers { get; init; } = SyntaxList<Hlsl, ArrayRank>.Empty;
        public CloseParenToken             closeParenToken     { get; init; } = new();
        public Expression                  expression          { get; init; }

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { openParenToken, type, arrayRankSpecifiers, closeParenToken, expression };
    }
}
