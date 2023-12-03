namespace me.tooster.sdf.AST.Shaderlab.Syntax.Commands {
    [SyntaxNode] public partial record ZClip : Command {
        public ZClipKeyword    zClipKeyword { get; init; } = new();
        public CommandArgument enabled      { get; init; }
    }
}
