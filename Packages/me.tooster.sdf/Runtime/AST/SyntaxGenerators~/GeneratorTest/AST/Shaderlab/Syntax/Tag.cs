using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax {
    [Syntax] public partial record  Tag : Syntax<Shaderlab> {
        private readonly  QuotedStringLiteral _key   ;
        private readonly  QuotedStringLiteral _value ;
    }
}
