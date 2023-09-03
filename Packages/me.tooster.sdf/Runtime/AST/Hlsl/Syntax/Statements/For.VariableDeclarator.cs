#nullable enable
using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
   public partial record For {
       [Syntax] public partial record VariableDeclarator : Initializer {
            public VariableDeclarator declarator { get; internal init; }
        }
    }
}
