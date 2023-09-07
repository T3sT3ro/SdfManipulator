using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl {
    /// Marker interface for Hlsl language
    public interface Hlsl { }

    [Syntax] public abstract partial record Expression : Syntax<Hlsl>;

    [Syntax] public abstract partial record Statement : Syntax<Hlsl>;
}
