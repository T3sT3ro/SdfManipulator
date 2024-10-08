namespace me.tooster.sdf.AST.Shaderlab.Syntax.Commands {
    [SyntaxNode] public partial record ZTest : Command {
        public ZTestKeyword    zTestKeyword { get; init; } = new();
        public CommandArgument operation    { get; init; }
    }
}
