using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific {
    // Fallback "shader"
    [AstSyntax] public partial record Fallback : ShaderStatement {
        public FallbackKeyword     fallbackKeyword { get; init; } = new();
        public        QuotedStringLiteral name { get; init; }
    }
}
