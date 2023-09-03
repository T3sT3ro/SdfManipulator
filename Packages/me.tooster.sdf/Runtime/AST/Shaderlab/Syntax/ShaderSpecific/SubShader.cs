using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific {
    // SubShader { ... }
    /// <a href="https://docs.unity3d.com/Manual/SL-SubShader.html">SubShader</a>
    [Syntax] public partial record SubShader : ShaderStatement {
        [Init] private readonly SubShaderKeyword                                _subShaderKeyword;
        [Init] private readonly OpenBraceToken                                  _openBraceToken;
        [Init] private readonly SyntaxList<Shaderlab, SubShaderOrPassStatement> _statements;
        [Init] private readonly CloseBraceToken                                 _closeBraceToken;
    }
}
