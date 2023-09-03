using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific {
    // Properties { \n...\n }
    /// <a href="https://docs.unity3d.com/Manual/SL-Properties.html">Properties</a>
    public record MaterialProperties : ShaderStatement {
        public PropertiesKeyword               _propertiesKeyword { get; init; } = new();
        public OpenBraceToken                  _openBraceToken    { get; init; } = new();
        public SyntaxList<Shaderlab, Property> _properties        { get; init; } = new();
        public CloseBraceToken                 _closeBraceToken   { get; init; } = new();

        public override IReadOnlyList<SyntaxOrToken<Shaderlab>> ChildNodesAndTokens => new SyntaxOrToken<Shaderlab>[]
            { propertiesKeyword, openBraceToken, properties, closeBraceToken };
    }
}
