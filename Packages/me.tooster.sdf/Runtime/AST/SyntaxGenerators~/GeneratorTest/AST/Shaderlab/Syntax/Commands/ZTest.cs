using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax.Commands {
    [AstSyntax] public partial record ZTest : Command {
        public ZTestKeyword    zTestKeyword { get; init; } = new();
        public        CommandArgument operation { get; init; }
    }
}
