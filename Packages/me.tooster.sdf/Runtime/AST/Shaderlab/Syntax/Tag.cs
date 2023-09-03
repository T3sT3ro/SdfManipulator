using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax {
    public record Tag : Syntax<Shaderlab> {
        public QuotedStringLiteral _key   { get; init; }
        public QuotedStringLiteral _value { get; init; }

        public override IReadOnlyList<SyntaxOrToken<Shaderlab>> ChildNodesAndTokens => new[]
            { key, value };
    }
}
