#nullable enable
using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax.Commands {
    [AstSyntax] public partial record Cull : Command {
        public CullKeyword     cullKeyword { get; init; } = new();
        public        CommandArgument state { get; init; }
    }
}
