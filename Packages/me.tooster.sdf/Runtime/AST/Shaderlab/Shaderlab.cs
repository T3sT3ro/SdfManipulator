using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab {
    /// marker interface for the language
    public interface Shaderlab { }
    
    /// statement occuring only in the shader block
    public abstract record ShaderStatement : Syntax<Shaderlab>;
    /// statement occuring either in a subshader or a pass block
    public abstract record SubShaderOrPassStatement : Syntax<Shaderlab>;
    /// statement occuring only inside subshader block
    public abstract record SubShaderStatement : SubShaderOrPassStatement;
    /// statement occuting only inside pass block
    public abstract record PassStatement : SubShaderOrPassStatement;
    
    /// Predefined command with arguments
    public abstract record Command : SubShaderOrPassStatement;
}
