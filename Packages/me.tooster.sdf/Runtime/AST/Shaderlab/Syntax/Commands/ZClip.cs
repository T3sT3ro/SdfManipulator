using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax.Commands {
    [Syntax] public partial record ZClip : Command {
        [Init] private readonly ZClipKeyword    _zClipKeyword;
        private readonly        CommandArgument _enabled;
    }
}
