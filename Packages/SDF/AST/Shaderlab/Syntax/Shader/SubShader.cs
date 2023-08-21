using System.Collections.Generic;
using System.Linq;

namespace AST.Shaderlab.Syntax.Shader {
    /// <a href="https://docs.unity3d.com/Manual/SL-SubShader.html">SubShader</a>
    public record SubShader : ShaderStatement {
        public SubShaderKeyword                        subShaderKeyword { get; set; } = new();
        public OpenBraceToken                          openBraceToken   { get; set; } = new();
        public IReadOnlyList<SubShaderOrPassStatement> statements       { get; set; }
        public CloseBraceToken                         closeBraceToken  { get; set; } = new();


        public override IReadOnlyList<IShaderlabSyntaxOrToken> ChildNodesAndTokens => new IShaderlabSyntaxOrToken[]
                { subShaderKeyword, openBraceToken }
            .Concat(statements)
            .Append(closeBraceToken)
            .ToList();
    }
}
