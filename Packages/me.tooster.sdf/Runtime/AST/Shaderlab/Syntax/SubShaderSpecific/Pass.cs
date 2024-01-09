using me.tooster.sdf.AST.Syntax;
using me.tooster.sdf.AST.Syntax.CommonSyntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax.SubShaderSpecific {
    /// <a href="https://docs.unity3d.com/Manual/SL-Pass.html">Pass</a> 
    [SyntaxNode] public partial record Pass : SubShaderStatement {
        public PassKeyword                          passKeyword     { get; init; } = new();
        public OpenBraceToken                       openBraceToken  { get; init; } = new();
        public SyntaxList<shaderlab, PassStatement> statements      { get; init; } = new();
        public CloseBraceToken                      closeBraceToken { get; init; } = new();
    }
}
