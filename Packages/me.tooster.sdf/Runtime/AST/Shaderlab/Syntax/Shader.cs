#nullable enable
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax {
    // Shader "MyShader" { ... }
    /// <a href="https://docs.unity3d.com/Manual/SL-Shader.html">Shader</a>
    /// <a href="https://docs.unity3d.com/Manual/SL-Fallback.html">Fallback</a>
    /// <a href="https://docs.unity3d.com/Manual/SL-CustomEditor.html">CustomEditor</a>
    public record Shader : Syntax<Shaderlab> {
        public ShaderKeyword                          _shaderKeyword      { get; init; } = new();
        public QuotedStringLiteral                    _name               { get; init; }
        public OpenBraceToken                         _openBraceToken     { get; init; } = new();
        public MaterialProperties?                    _materialProperties { get; init; }
        public SyntaxList<Shaderlab, ShaderStatement> _shaderStatements   { get; init; } = new();
        public CloseBraceToken                        _closeBraceToken    { get; init; } = new();

        public override IReadOnlyList<SyntaxOrToken<Shaderlab>> ChildNodesAndTokens => new SyntaxOrToken<Shaderlab>?[]
                { shaderKeyword, name, openBraceToken, materialProperties, shaderStatements, closeBraceToken }
            .FilterNotNull().ToList();
    }
}
