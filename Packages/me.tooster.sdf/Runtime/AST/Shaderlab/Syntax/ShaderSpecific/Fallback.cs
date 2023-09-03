using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific {
    // Fallback "shader"
    [Syntax] public partial record Fallback : ShaderStatement {
        [Init] private readonly FallbackKeyword     _fallbackKeyword;
        private readonly        QuotedStringLiteral _name;
    }
}
