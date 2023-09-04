#nullable enable
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific {
    // [Gamma]
    // [Header(header title)]
    // [Enum(One,1,SrcAlpha,5)]
    // [PowerSlider(3.0)]
    [Syntax] public partial record Attribute : Syntax<Shaderlab> {
        [Init] private readonly OpenBracketToken              _openBracketToken;
        private readonly        AttributeToken                _attributeToken;
        private readonly        ArgumentList<AttributeValue>? _arguments;
        [Init] private readonly CloseBracketToken             _closeBracketToken;

        [Syntax] public partial record AttributeValue : Syntax<Shaderlab> {
            private readonly AttributeStringLiteral _value;
        }
    }
}
