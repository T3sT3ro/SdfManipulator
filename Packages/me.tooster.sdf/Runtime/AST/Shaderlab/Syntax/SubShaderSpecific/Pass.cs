using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax.SubShaderSpecific {
    /// <a href="https://docs.unity3d.com/Manual/SL-Pass.html">Pass</a> 
    [Syntax] public partial record Pass : SubShaderStatement {
        [Init] private readonly PassKeyword                          _passKeyword;
        [Init] private readonly OpenBraceToken                       _openBraceToken;
        [Init] private readonly SyntaxList<Shaderlab, PassStatement> _statements;
        [Init] private readonly CloseBraceToken                      _closeBraceToken;
    }
}
