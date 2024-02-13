using Expression = me.tooster.sdf.AST.Syntax.CommonSyntax.Expression<me.tooster.sdf.AST.hlsl>;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Expressions {
    // {EXPR, EPXR, ...}
    [SyntaxNode] public partial record BracedInitializerExpression : Expression {
        public BracedList<Expression> components { get; init; } = new();
        
        public static implicit operator BracedInitializerExpression(Expression[] e) => new BracedInitializerExpression { components = e };
        public static implicit operator BracedInitializerExpression(BracedList<Expression> list) => new BracedInitializerExpression { components = list };
    }
}
