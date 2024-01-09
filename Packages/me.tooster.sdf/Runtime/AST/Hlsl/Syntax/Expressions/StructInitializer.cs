using Expression = me.tooster.sdf.AST.Syntax.CommonSyntax.Expression<me.tooster.sdf.AST.hlsl>;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Expressions {
    // {EXPR, EPXR, ...}
    [SyntaxNode] public partial record StructInitializer : Expression {
        public BracedList<Expression> components { get; init; } = new();
    }
}
