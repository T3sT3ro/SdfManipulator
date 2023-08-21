using System.Collections.Generic;
using System.Linq;

namespace AST.Shaderlab.Syntax.Shader {
    /// <a href="https://docs.unity3d.com/Manual/SL-Properties.html">Properties</a>
    public record MaterialProperties : ShaderStatement {
        public PropertiesKeyword       propertiesKeyword { get; set; } = new();
        public OpenBraceToken          openBraceToken    { get; set; } = new();
        public IReadOnlyList<Property> properties        { get; set; }
        public CloseBraceToken         closeBraceToken   { get; set; } = new();

        public override IReadOnlyList<IShaderlabSyntaxOrToken> ChildNodesAndTokens => new IShaderlabSyntaxOrToken[]
                { propertiesKeyword, openBraceToken }
            .Concat(properties)
            .Append(closeBraceToken)
            .ToList();
    }
}
