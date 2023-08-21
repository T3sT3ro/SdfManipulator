using System.Collections.Generic;
using System.Linq;

namespace AST.Shaderlab.Syntax.SubShader {
    /// <a href="https://docs.unity3d.com/Manual/SL-Pass.html">Pass</a> 
    public partial record Pass : SubShaderStatement {
        public PassKeyword                  passKeyword     { get; set; } = new();
        public OpenBraceToken               openBraceToken  { get; set; } = new();
        public IReadOnlyList<PassStatement> statements      { get; set; }
        public CloseBraceToken              closeBraceToken { get; set; } = new();

        public override IReadOnlyList<IShaderlabSyntaxOrToken> ChildNodesAndTokens => new IShaderlabSyntaxOrToken[]
                { passKeyword, openBraceToken }
            .Concat(statements)
            .Append(closeBraceToken)
            .ToList();
    }
}
