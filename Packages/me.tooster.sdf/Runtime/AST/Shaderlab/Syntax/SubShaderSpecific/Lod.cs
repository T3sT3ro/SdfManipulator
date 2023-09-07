using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax.SubShaderSpecific {
    // LOD 100
    /// <a href="https://docs.unity3d.com/Manual/SL-ShaderLOD.html">LOD</a>
    [Syntax] public partial record Lod : SubShaderStatement {
        [Init] private readonly LodKeyword _lodKeyword;
        private readonly        IntLiteral _level;
    }
}
