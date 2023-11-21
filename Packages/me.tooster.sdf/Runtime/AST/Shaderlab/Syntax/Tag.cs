using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax {
    [SyntaxNode] public partial record Tag {
        public QuotedStringLiteral key   { get; init; }
        public QuotedStringLiteral value { get; init; }
    }
}
