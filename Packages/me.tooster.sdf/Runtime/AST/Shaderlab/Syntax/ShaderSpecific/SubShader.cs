using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific {
    // SubShader { ... }
    /// <a href="https://docs.unity3d.com/Manual/SL-SubShader.html">SubShader</a>
    [SyntaxNode] public partial record SubShader : ShaderStatement {
        public SubShaderKeyword                                                 subShaderKeyword { get; init; } = new();
        public OpenBraceToken                                                   openBraceToken   { get; init; } = new();
        public SyntaxList<shaderlab, SubShaderOrPassStatement> statements       { get; init; } = new();
        public CloseBraceToken                                                  closeBraceToken  { get; init; } = new();
    }
}
