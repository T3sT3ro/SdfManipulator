using System.Collections.Generic;
using AST.Syntax;

namespace AST.Shaderlab.Syntax.SubShaderSpecific {
    /// <a href="https://docs.unity3d.com/Manual/SL-Pass.html">Pass</a> 
    public partial record Pass : SubShaderStatement {
        public PassKeyword                          passKeyword     { get; init; } = new();
        public OpenBraceToken                       openBraceToken  { get; init; } = new();
        public SyntaxList<Shaderlab, PassStatement> statements      { get; init; } = new();
        public CloseBraceToken                      closeBraceToken { get; init; } = new();

        public override IReadOnlyList<SyntaxOrToken<Shaderlab>> ChildNodesAndTokens => new SyntaxOrToken<Shaderlab>[]
            { passKeyword, openBraceToken, statements, closeBraceToken };
    }
}