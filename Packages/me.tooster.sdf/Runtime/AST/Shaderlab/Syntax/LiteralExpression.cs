using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax {
    [SyntaxNode] public partial record LiteralExpression : Syntax<shaderlab> {
        public Literal<shaderlab> literal { get; init; }
    }
}
