using me.tooster.sdf.AST.Syntax.CommonSyntax;
namespace me.tooster.sdf.AST.Shaderlab.Syntax.SubShaderSpecific {
    /// <a href="https://docs.unity3d.com/Manual/SL-Pass.html">Pass</a>
    [SyntaxNode] public partial record Pass : SubShaderStatement {
        public PassKeyword                          passKeyword     { get; init; } = new();
        public OpenBraceToken                       openBraceToken  { get; init; } = new();
        // tags and commands can stand in a pass as well as in a subshader
        public SyntaxList<shaderlab, SubShaderOrPassStatement> statements      { get; init; } = new();
        public CloseBraceToken                      closeBraceToken { get; init; } = new();
    }
}
