using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific {
    // [Gamma] Property ("_Gamma", Float) = 1
    /// <a href="https://docs.unity3d.com/2023.2/Documentation/Manual/SL-Properties.html">Shader</a>
    public partial record Property : Syntax<Shaderlab> {
        public SyntaxList<Shaderlab, Attribute> attributes      { get; init; } = new();
        public IdentifierToken                  id              { get; init; }
        public OpenParenToken                   openParenToken  { get; init; } = new();
        public QuotedStringLiteral              displayName     { get; init; }
        public CommaToken                       commaToken      { get; init; } = new();
        public PropertyType                     propertyType    { get; init; }
        public CloseParenToken                  closeParenToken { get; init; } = new();
        public Initializer                      initializer     { get; init; }

        public override IReadOnlyList<SyntaxOrToken<Shaderlab>> ChildNodesAndTokens => new SyntaxOrToken<Shaderlab>[]
            { attributes, id, openParenToken, displayName, commaToken, propertyType, closeParenToken, initializer };
    }
}
