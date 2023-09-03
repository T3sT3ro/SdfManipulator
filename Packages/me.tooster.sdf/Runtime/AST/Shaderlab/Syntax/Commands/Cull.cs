#nullable enable
using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax.Commands {
    [Syntax] public partial record Cull : Command {
        [Init] private readonly CullKeyword     _cullKeyword;
        private readonly        CommandArgument _state;
    }
}
