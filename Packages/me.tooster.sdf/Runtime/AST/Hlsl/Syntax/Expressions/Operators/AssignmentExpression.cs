using Expression = me.tooster.sdf.AST.Syntax.CommonSyntax.Expression<me.tooster.sdf.AST.hlsl>;
namespace me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators {
    // x = y
    // y <<= 2
    [SyntaxNode] public partial record AssignmentExpression : Expression {
        public Expression      left            { get; init; }
        public AssignmentToken assignmentToken { get; init; } = new EqualsToken();
        public Expression      right           { get; init; }
    }
}
