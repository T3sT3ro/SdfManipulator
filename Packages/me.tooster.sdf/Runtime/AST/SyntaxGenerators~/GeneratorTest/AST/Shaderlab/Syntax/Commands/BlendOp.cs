using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax.Commands {
    [AstSyntax] public partial record BlendOp : Command {
        public BlendOpKeyword  blendOpKeyword { get; init; } = new();
        public        CommandArgument operationArg { get; init; }
    }
}
