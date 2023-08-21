#nullable enable
using System.Collections.Generic;
using System.Linq;
using AST.Syntax;

namespace AST.Shaderlab.Syntax.Shader {
    // [Gamma]
    // [Header(header title)]
    // [Enum(One,1,SrcAlpha,5)]
    // [PowerSlider(3.0)]
    public record Attribute : ShaderlabSyntax {
        public OpenBracketToken              openBracketToken  { get; set; } = new();
        public AttributeToken                attributeToken    { get; set; }
        public ArgumentList<AttributeValue>? arguments         { get; set; }
        public CloseBracketToken             closeBracketToken { get; set; } = new();

        public override IReadOnlyList<IShaderlabSyntaxOrToken> ChildNodesAndTokens => new IShaderlabSyntaxOrToken[]
                { openBracketToken, attributeToken, arguments, closeBracketToken }
            .FilterNotNull().ToList();

        public record AttributeValue : ShaderlabSyntax {
            public AttributeStringLiteral value { get; set; }

            public override IReadOnlyList<IShaderlabSyntaxOrToken> ChildNodesAndTokens => new[] { value };
        }
    }
}
