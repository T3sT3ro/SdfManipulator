using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax.Commands {
    [SyntaxNode] public partial record ZWrite : Command {
        public ZWriteKeyword   zWriteKeyword { get; init; } = new();
        public CommandArgument state         { get; init; }
    }
}
