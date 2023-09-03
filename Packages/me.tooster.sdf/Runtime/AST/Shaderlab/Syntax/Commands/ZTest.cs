using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax.Commands {
    [Syntax] public partial record ZTest : Command {
        [Init] private readonly ZTestKeyword    _zTestKeyword;
        private readonly        CommandArgument _operation;
    }
}
