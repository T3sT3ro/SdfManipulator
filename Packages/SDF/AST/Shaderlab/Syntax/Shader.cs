#nullable enable
using System.Collections.Generic;
using System.Linq;
using AST.Shaderlab.Syntax.ShaderSpecific;
using AST.Syntax;

namespace AST.Shaderlab.Syntax {
    // Shader "MyShader" { ... }
    /// <a href="https://docs.unity3d.com/Manual/SL-Shader.html">Shader</a>
    /// <a href="https://docs.unity3d.com/Manual/SL-Fallback.html">Fallback</a>
    /// <a href="https://docs.unity3d.com/Manual/SL-CustomEditor.html">CustomEditor</a>
    public record Shader : Syntax<Shaderlab> {
        public ShaderKeyword       shaderKeyword      { get; init; } = new();
        public QuotedStringLiteral name               { get; init; }
        public OpenBraceToken      openBraceToken     { get; init; } = new();
        public MaterialProperties? materialProperties { get; init; }

        public SyntaxList<Shaderlab, ShaderStatement> shaderStatements { get; init; } =
            SyntaxList<Shaderlab, ShaderStatement>.Empty;

        public CloseBraceToken closeBraceToken { get; init; } = new();

        public override IReadOnlyList<SyntaxOrToken<Shaderlab>> ChildNodesAndTokens => new SyntaxOrToken<Shaderlab>?[]
                { shaderKeyword, name, openBraceToken, materialProperties, shaderStatements, closeBraceToken }
            .FilterNotNull().ToList();
    }
}
