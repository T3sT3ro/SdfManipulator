using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific {
    // [Gamma] Property ("_Gamma", Float) = 1
    /// <a href="https://docs.unity3d.com/2023.2/Documentation/Manual/SL-Properties.html">Shader</a>
    public partial record Property : Syntax<Shaderlab> {
        public SyntaxList<Shaderlab, Attribute> _attributes      { get; init; } = new();
        public IdentifierToken                  _id              { get; init; }
        public OpenParenToken                   _openParenToken  { get; init; } = new();
        public QuotedStringLiteral              _displayName     { get; init; }
        public CommaToken                       _commaToken      { get; init; } = new();
        public PropertyType                     _propertyType    { get; init; }
        public CloseParenToken                  _closeParenToken { get; init; } = new();
        public Initializer                      _initializer     { get; init; }

        public override IReadOnlyList<SyntaxOrToken<Shaderlab>> ChildNodesAndTokens => new SyntaxOrToken<Shaderlab>[]
            { attributes, id, openParenToken, displayName, commaToken, propertyType, closeParenToken, initializer };
    }
}
