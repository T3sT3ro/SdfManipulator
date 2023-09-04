using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific {
    // Properties { \n...\n }
    /// <a href="https://docs.unity3d.com/Manual/SL-Properties.html">Properties</a>
    [Syntax] public partial record MaterialProperties : ShaderStatement {
        [Init] private readonly PropertiesKeyword               _propertiesKeyword;
        [Init] private readonly OpenBraceToken                  _openBraceToken;
        [Init] private readonly SyntaxList<Shaderlab, Property> _properties;
        [Init] private readonly CloseBraceToken                 _closeBraceToken;
    }
}
