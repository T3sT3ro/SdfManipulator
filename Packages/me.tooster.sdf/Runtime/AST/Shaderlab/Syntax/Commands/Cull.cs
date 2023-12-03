#nullable enable
namespace me.tooster.sdf.AST.Shaderlab.Syntax.Commands {
    [SyntaxNode] public partial record Cull : Command {
        public CullKeyword     cullKeyword { get; init; } = new();
        public CommandArgument state       { get; init; }
    }
}
