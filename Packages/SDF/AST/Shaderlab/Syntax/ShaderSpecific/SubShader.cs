using System.Collections.Generic;
using System.Linq;
using AST.Syntax;

namespace AST.Shaderlab.Syntax.ShaderSpecific {
    // SubShader { ... }
    /// <a href="https://docs.unity3d.com/Manual/SL-SubShader.html">SubShader</a>
    public record SubShader : ShaderStatement {
        public SubShaderKeyword                                subShaderKeyword { get; init; } = new();
        public OpenBraceToken                                  openBraceToken   { get; init; } = new();
        public SyntaxList<Shaderlab, SubShaderOrPassStatement> statements       { get; init; } = SyntaxList<Shaderlab, SubShaderOrPassStatement>.Empty;
        public CloseBraceToken                                 closeBraceToken  { get; init; } = new();


        public override IReadOnlyList<SyntaxOrToken<Shaderlab>> ChildNodesAndTokens => new SyntaxOrToken<Shaderlab>[]
                { subShaderKeyword, openBraceToken, statements, closeBraceToken };
    }
}
