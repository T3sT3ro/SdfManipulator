using Expression = me.tooster.sdf.AST.Syntax.CommonSyntax.Expression<me.tooster.sdf.AST.hlsl>;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators {
    [SyntaxNode] public partial record Ternary : Expression {
        public Expression    condition     { get; init; }
        public QuestionToken questionToken { get; init; } = new();
        public Expression    whenTrue      { get; init; }
        public ColonToken    colonToken    { get; init; } = new();
        public Expression    whenFalse     { get; init; }
    }
}
