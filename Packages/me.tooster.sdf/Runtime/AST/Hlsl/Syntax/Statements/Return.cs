#nullable enable
namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    [SyntaxNode] public partial record Return : Statement {
        public ReturnKeyword returnKeyword { get; init; } = new();
        public Expression?   expression    { get; init; }
    }
}
