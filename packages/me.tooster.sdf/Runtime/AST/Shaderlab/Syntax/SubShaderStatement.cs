namespace me.tooster.sdf.AST.Shaderlab.Syntax {
    /// statement occuring only inside subshader block
    [SyntaxNode] public abstract partial record SubShaderStatement : SubShaderOrPassStatement;
}
