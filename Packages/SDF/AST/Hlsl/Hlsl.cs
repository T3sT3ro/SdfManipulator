using AST.Syntax;

namespace AST.Hlsl {
    /// Marker interface for Hlsl language
    public interface Hlsl { }
    
    public abstract record Expression : Syntax<Hlsl>;
    public abstract record Statement : Syntax<Hlsl>;
}
