using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific {
    // [Gamma] Property ("_Gamma", Float) = 1
    /// <a href="https://docs.unity3d.com/2023.2/Documentation/Manual/SL-Properties.html">Shader</a>
    [Syntax] public partial record Property : Syntax<Shaderlab> {
        [Init] private readonly SyntaxList<Shaderlab, Attribute> _attributes;
        private readonly        IdentifierToken                  _id;
        [Init] private readonly OpenParenToken                   _openParenToken;
        private readonly        QuotedStringLiteral              _displayName;
        [Init] private readonly CommaToken                       _commaToken;
        private readonly        Type                             _propertyType;
        [Init] private readonly CloseParenToken                  _closeParenToken;
        private readonly        Initializer                      _initializer;
    }
}
