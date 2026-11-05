using Expression = me.tooster.sdf.AST.Syntax.CommonSyntax.Expression<me.tooster.sdf.AST.hlsl>;
using Statement = me.tooster.sdf.AST.Syntax.CommonSyntax.Statement<me.tooster.sdf.AST.hlsl>;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    [SyntaxNode] public partial record Return : Statement {
        public ReturnKeyword  returnKeyword  { get; init; } = new();
        public Expression     expression     { get; init; }
        public SemicolonToken semicolonToken { get; init; } = new();

        public static implicit operator Return(Expression e) => new() { expression = e };
    }
}
