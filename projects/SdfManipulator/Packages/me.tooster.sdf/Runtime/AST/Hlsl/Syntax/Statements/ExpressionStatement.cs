#nullable enable
using Expression = me.tooster.sdf.AST.Syntax.CommonSyntax.Expression<me.tooster.sdf.AST.hlsl>;
using Statement = me.tooster.sdf.AST.Syntax.CommonSyntax.Statement<me.tooster.sdf.AST.hlsl>;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    // expression;
    // ; <- empty expression statement
    [SyntaxNode] public partial record ExpressionStatement : Statement {
        public Expression?    expression     { get; init; }
        public SemicolonToken semicolonToken { get; init; } = new();

        public static implicit operator ExpressionStatement(Expression e) => new() { expression = e };
    }
}
