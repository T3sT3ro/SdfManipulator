using System.Collections.Generic;

namespace AST.Shaderlab.Syntax.Shader {
    public record Fallback : ShaderStatement {
        public FallbackKeyword     fallbackKeyword { get; set; } = new();
        public QuotedStringLiteral name            { get; set; }

        public override IReadOnlyList<IShaderlabSyntaxOrToken> ChildNodesAndTokens => new ShaderlabToken[]
            { fallbackKeyword, name };
    }
}
