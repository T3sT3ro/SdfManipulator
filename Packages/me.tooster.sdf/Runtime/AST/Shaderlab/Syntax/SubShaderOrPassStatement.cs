using me.tooster.sdf.AST;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax {
    /// statement occuring either in a subshader or a pass block
    [SyntaxNode] public abstract partial record SubShaderOrPassStatement : Syntax<shaderlab>;
}
