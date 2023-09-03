using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax.Commands {
    [Syntax] public partial record  ZWrite : Command {
        [Init] private readonly ZWriteKeyword   _zWriteKeyword ;
        private readonly        CommandArgument _state         ;

    }
}
