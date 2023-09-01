using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl {
    /// Marker interface for Hlsl language
    public interface Hlsl { }
    
    public abstract partial record Expression : Syntax<Hlsl>;
    public abstract partial record Statement : Syntax<Hlsl>;
}
