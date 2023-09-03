using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax.SubShaderSpecific {
    /// <a href="https://docs.unity3d.com/Manual/SL-Pass.html">Pass</a> 
    public partial record Pass : SubShaderStatement {
        public PassKeyword                          _passKeyword     { get; init; } = new();
        public OpenBraceToken                       _openBraceToken  { get; init; } = new();
        public SyntaxList<Shaderlab, PassStatement> _statements      { get; init; } = new();
        public CloseBraceToken                      _closeBraceToken { get; init; } = new();

        public override IReadOnlyList<SyntaxOrToken<Shaderlab>> ChildNodesAndTokens => new SyntaxOrToken<Shaderlab>[]
            { passKeyword, openBraceToken, statements, closeBraceToken };
    }
}