using System.Collections.Generic;
using System.Linq;
using AST.Syntax;

namespace AST.Shaderlab.Syntax.ShaderSpecific {
    // Properties { \n...\n }
    /// <a href="https://docs.unity3d.com/Manual/SL-Properties.html">Properties</a>
    public record MaterialProperties : ShaderStatement {
        public PropertiesKeyword               propertiesKeyword { get; init; } = new();
        public OpenBraceToken                  openBraceToken    { get; init; } = new();
        public SyntaxList<Shaderlab, Property> properties        { get; init; } = new();
        public CloseBraceToken                 closeBraceToken   { get; init; } = new();

        public override IReadOnlyList<SyntaxOrToken<Shaderlab>> ChildNodesAndTokens => new SyntaxOrToken<Shaderlab>[]
            { propertiesKeyword, openBraceToken, properties, closeBraceToken };
    }
}
