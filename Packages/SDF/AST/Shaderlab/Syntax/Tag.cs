using System.Collections.Generic;

namespace AST.Shaderlab.Syntax {
    public record Tag : ShaderlabSyntax {
        public QuotedStringLiteral key   { get; set; }
        public QuotedStringLiteral value { get; set; }

        public override IReadOnlyList<IShaderlabSyntaxOrToken> ChildNodesAndTokens => new[] { key, value };
    }
}
