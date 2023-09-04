using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax.SubShaderSpecific {
    public partial record Pass {
        [Syntax] public partial record  Name : PassStatement {
            [Init] private readonly NameKeyword         _nameKeyword ;
            private readonly        QuotedStringLiteral _name        ;
        }
    }
}
