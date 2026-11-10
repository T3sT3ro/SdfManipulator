using Statement = me.tooster.sdf.AST.Syntax.CommonSyntax.Statement<me.tooster.sdf.AST.hlsl>;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    // typedef float3 position;
    [SyntaxNode] public partial record Typedef : Statement {
        public TypedefKeyword typedefKeyword { get; init; } = new();
        public Type           type           { get; init; }
        public Identifier     id             { get; init; }
        public SemicolonToken semicolonToken { get; init; } = new();
    }
}
