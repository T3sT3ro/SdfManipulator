using System.Collections.Generic;

namespace AST.Shaderlab.Syntax.SubShader {
    /// <a href="https://docs.unity3d.com/Manual/SL-ShaderLOD.html">LOD</a>
    public record Lod : SubShaderStatement {
        public LodKeyword lodKeyword { get; set; } = new();
        public IntLiteral level      { get; set; }

        public override IReadOnlyList<IShaderlabSyntaxOrToken> ChildNodesAndTokens => new IShaderlabSyntaxOrToken[]
            { lodKeyword, level };
    }
}
