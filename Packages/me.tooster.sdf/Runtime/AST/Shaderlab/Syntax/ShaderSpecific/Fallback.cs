using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific {
    // Fallback "shader"
    public record Fallback : ShaderStatement {
        public FallbackKeyword     _fallbackKeyword { get; init; } = new();
        public QuotedStringLiteral _name            { get; init; }

        public override IReadOnlyList<SyntaxOrToken<Shaderlab>> ChildNodesAndTokens => new Token<Shaderlab>[]
            { fallbackKeyword, name };
    }
}
