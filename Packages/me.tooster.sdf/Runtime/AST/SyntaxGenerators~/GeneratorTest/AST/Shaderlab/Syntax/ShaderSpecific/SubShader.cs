using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific {
    // SubShader { ... }
    /// <a href="https://docs.unity3d.com/Manual/SL-SubShader.html">SubShader</a>
    [AstSyntax] public partial record SubShader : ShaderStatement {
        public SubShaderKeyword                                subShaderKeyword { get; init; } = new();
        public OpenBraceToken                                  openBraceToken { get; init; } = new();
        public SyntaxList<Shaderlab, SubShaderOrPassStatement> statements { get; init; } = new();
        public CloseBraceToken                                 closeBraceToken { get; init; } = new();
    }
}
