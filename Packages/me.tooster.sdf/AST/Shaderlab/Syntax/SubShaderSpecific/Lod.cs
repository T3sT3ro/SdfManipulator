using System.Collections.Generic;
using AST.Syntax;

namespace AST.Shaderlab.Syntax.SubShaderSpecific {
    // LOD 100
    /// <a href="https://docs.unity3d.com/Manual/SL-ShaderLOD.html">LOD</a>
    public record Lod : SubShaderStatement {
        public LodKeyword lodKeyword { get; init; } = new();
        public IntLiteral level      { get; init; }

        public override IReadOnlyList<SyntaxOrToken<Shaderlab>> ChildNodesAndTokens => new SyntaxOrToken<Shaderlab>[]
            { lodKeyword, level };
    }
}
