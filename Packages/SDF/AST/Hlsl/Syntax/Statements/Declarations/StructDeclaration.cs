#nullable enable
using System.Collections.Generic;
using System.Linq;

namespace AST.Hlsl.Syntax.Statements.Declarations {
    public record StructDeclaration : Statement {
        public Type.UserDefined type { get; set; }

        public IReadOnlyList<Member> members { get; set; }

        public record Member : HlslSyntax {
            public HlslToken?     interpolation { get; set; }
            public Type           type          { get; set; }
            public IdentifierName id            { get; set; }
            public Semantic?      semantic      { get; set; }

            public override IReadOnlyList<HlslSyntax> ChildNodes => new HlslSyntax[] { type, id, semantic };

            public override IReadOnlyList<HlslSyntaxOrToken> ChildNodesAndTokens =>
                new HlslSyntaxOrToken[] { interpolation, type, id, semantic };
        }

        public override IReadOnlyList<HlslSyntax> ChildNodes => new HlslSyntax[] { type }.Concat(members).ToArray();
        public override IReadOnlyList<HlslSyntaxOrToken> ChildNodesAndTokens => ChildNodes;
    }
}
