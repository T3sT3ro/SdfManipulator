using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific {
    // Properties { \n...\n }
    /// <a href="https://docs.unity3d.com/Manual/SL-Properties.html">Properties</a>
    [AstSyntax] public partial record MaterialProperties : ShaderStatement {
        public PropertiesKeyword               propertiesKeyword { get; init; } = new();
        public OpenBraceToken                  openBraceToken { get; init; } = new();
        public SyntaxList<Shaderlab, Property> properties { get; init; } = new();
        public CloseBraceToken                 closeBraceToken { get; init; } = new();
    }
}
