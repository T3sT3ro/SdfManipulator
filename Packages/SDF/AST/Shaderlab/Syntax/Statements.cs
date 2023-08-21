namespace AST.Shaderlab.Syntax {
    // statement for shader only
    public abstract record ShaderStatement : ShaderlabSyntax;
    // statement for both subshader and pass
    public abstract record SubShaderOrPassStatement : ShaderlabSyntax;
    // statement for subshader only
    public abstract record SubShaderStatement : SubShaderOrPassStatement;
    // statement for pass only
    public abstract record PassStatement : SubShaderOrPassStatement;
}
