using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators {
    // (float)1
    // (float[1])1.0f
    [AstSyntax] public partial record Cast : Expression {
        public OpenParenToken              openParenToken { get; init; } = new();
        public        Type                        type { get; init; }
        public SyntaxList<Hlsl, ArrayRank> arrayRankSpecifiers { get; init; } = new();
        public CloseParenToken             closeParenToken { get; init; } = new();
        public        Expression                  expression { get; init; }
    }
}
