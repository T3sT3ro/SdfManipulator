#nullable enable
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific {
    // [Gamma]
    // [Header(header title)]
    // [Enum(One,1,SrcAlpha,5)]
    // [PowerSlider(3.0)]
    [Syntax]
    public partial record Attribute : Syntax<Shaderlab> {
        public OpenBracketToken              _openBracketToken  { get; init; } = new();
        public AttributeToken                _attributeToken    { get; init; }
        public ArgumentList<AttributeValue>? _arguments         { get; init; }
        public CloseBracketToken             _closeBracketToken { get; init; } = new();

        [Syntax]
        public partial record AttributeValue : Syntax<Shaderlab> {
            public AttributeStringLiteral _value { get; init; }
        }
    }
}
