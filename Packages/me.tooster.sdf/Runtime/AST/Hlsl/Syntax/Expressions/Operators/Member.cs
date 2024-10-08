using Expression = me.tooster.sdf.AST.Syntax.CommonSyntax.Expression<me.tooster.sdf.AST.hlsl>;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators {
    // a.b.c
    [SyntaxNode] public partial record Member : Expression {
        public Expression expression { get; init; }
        public DotToken   dotToken   { get; init; } = new();
        public Identifier member     { get; init; }
    }
}
