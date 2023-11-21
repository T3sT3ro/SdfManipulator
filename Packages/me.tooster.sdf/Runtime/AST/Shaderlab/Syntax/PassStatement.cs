using me.tooster.sdf.AST;

namespace me.tooster.sdf.AST.Shaderlab.Syntax {
    /// statement occuting only inside pass block
    [SyntaxNode] public abstract partial record PassStatement : SubShaderOrPassStatement;
}
