using me.tooster.sdf.AST.Syntax;
using me.tooster.sdf.AST.Syntax.CommonSyntax;
using Expression = me.tooster.sdf.AST.Syntax.CommonSyntax.Expression<me.tooster.sdf.AST.hlsl>;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators {
    // (float)1
    // (float[1])1.0f
    [SyntaxNode] public partial record Cast : Expression {
        public OpenParenToken              openParenToken      { get; init; } = new();
        public Type                        type                { get; init; }
        public SyntaxList<hlsl, ArrayRank> arrayRankSpecifiers { get; init; } = new();
        public CloseParenToken             closeParenToken     { get; init; } = new();
        public Expression                  expression          { get; init; }
    }
}
