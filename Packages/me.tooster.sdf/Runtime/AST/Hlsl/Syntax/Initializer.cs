using Expression = me.tooster.sdf.AST.Syntax.CommonSyntax.Expression<me.tooster.sdf.AST.hlsl>;

namespace me.tooster.sdf.AST.Hlsl.Syntax {
    // = y
    // = {{a}, {b}}}
    [SyntaxNode] public partial record Initializer {
        public EqualsToken equalsToken { get; init; } = new();
        public Expression  value       { get; init; }
    }
}
