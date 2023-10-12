using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax {
    [AstSyntax] public partial record Tag : Syntax<Shaderlab> {
        public QuotedStringLiteral key { get; init; }
        public QuotedStringLiteral value { get; init; }
    }
}
