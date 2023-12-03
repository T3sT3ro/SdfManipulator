using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax {
    [SyntaxNode] public partial record LiteralExpression<T> : Syntax<shaderlab> where T : Literal {
        public Literal literal { get; init; }
    }
}
