namespace me.tooster.sdf.AST.Shaderlab.Syntax.Commands {
    [SyntaxNode] public partial record BlendOp : Command {
        public BlendOpKeyword  blendOpKeyword { get; init; } = new();
        public CommandArgument operationArg   { get; init; }
    }
}
