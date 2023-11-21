using me.tooster.sdf.AST;

namespace me.tooster.sdf.AST.Shaderlab.Syntax {
    /// Predefined command with arguments
    [SyntaxNode] public abstract partial record Command : SubShaderOrPassStatement;
}
