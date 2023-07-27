#nullable enable
using System;
using System.Collections.Generic;

namespace AST.Hlsl.Syntax {
    public abstract record Type : HlslSyntax {
        public record Predefined : Type {
            public HlslToken typeToken { get; set; }

            public override IReadOnlyList<HlslSyntax> ChildNodes => Array.Empty<HlslSyntax>();

            public override IReadOnlyList<HlslSyntaxOrToken> ChildNodesAndTokens =>
                new HlslSyntaxOrToken[] { typeToken };
        }

        public record UserDefined : Type {
            public IdentifierName id { get; set; }

            public override IReadOnlyList<HlslSyntax>        ChildNodes          => new HlslSyntax[] { id };
            public override IReadOnlyList<HlslSyntaxOrToken> ChildNodesAndTokens => ChildNodes;
        }
    }
}
