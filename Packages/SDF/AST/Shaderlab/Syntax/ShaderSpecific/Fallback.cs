using System.Collections.Generic;
using AST.Syntax;

namespace AST.Shaderlab.Syntax.ShaderSpecific {
    // Fallback "shader"
    public record Fallback : ShaderStatement {
        public FallbackKeyword     fallbackKeyword { get; init; } = new();
        public QuotedStringLiteral name            { get; init; }

        public override IReadOnlyList<SyntaxOrToken<Shaderlab>> ChildNodesAndTokens => new Token<Shaderlab>[]
            { fallbackKeyword, name };
    }
}
