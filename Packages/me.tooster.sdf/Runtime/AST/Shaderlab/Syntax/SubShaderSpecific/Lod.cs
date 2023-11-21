using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax.SubShaderSpecific {
    // LOD 100
    /// <a href="https://docs.unity3d.com/Manual/SL-ShaderLOD.html">LOD</a>
    [SyntaxNode] public partial record Lod : SubShaderStatement {
        public LodKeyword lodKeyword { get; init; } = new();
        public IntLiteral level      { get; init; }
    }
}
