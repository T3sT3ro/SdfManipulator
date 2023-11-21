using me.tooster.sdf.AST;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax {
    /// statement occuring only in the shader block
    [SyntaxNode] public abstract partial record ShaderStatement : Syntax<shaderlab>;
}
