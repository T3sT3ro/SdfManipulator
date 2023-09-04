#nullable enable
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
   [Syntax] public partial record Return : Statement {
        [Init] private readonly ReturnKeyword _returnKeyword;
        private readonly Expression?   _expression;
    }
}
