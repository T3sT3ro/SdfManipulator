using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax.Commands {
    [Syntax] public partial record BlendOp : Command {
        [Init] private readonly BlendOpKeyword  _blendOpKeyword;
        private readonly        CommandArgument _operationArg;
    }
}
