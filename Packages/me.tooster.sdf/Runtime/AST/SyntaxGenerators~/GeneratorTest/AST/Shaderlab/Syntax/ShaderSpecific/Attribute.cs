#nullable enable
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific {
    // [Gamma]
    // [Header(header title)]
    // [Enum(One,1,SrcAlpha,5)]
    // [PowerSlider(3.0)]
    [AstSyntax] public partial record Attribute : Syntax<Shaderlab> {
        public OpenBracketToken              openBracketToken { get; init; } = new();
        public        AttributeToken                attributeToken { get; init; }
        public        ArgumentList<AttributeValue>? arguments { get; init; }
        public CloseBracketToken             closeBracketToken { get; init; } = new();

        [AstSyntax] public partial record AttributeValue : Syntax<Shaderlab> {
            public AttributeStringLiteral value { get; init; }
        }
    }
}
