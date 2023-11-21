#nullable enable
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Shaderlab.Syntax;
using me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax {
    // Shader "MyShader" { ... }
    /// <a href="https://docs.unity3d.com/Manual/SL-Shader.html">Shader</a>
    /// <a href="https://docs.unity3d.com/Manual/SL-Fallback.html">Fallback</a>
    /// <a href="https://docs.unity3d.com/Manual/SL-CustomEditor.html">CustomEditor</a>
    [SyntaxNode] public partial record Shader {
        public ShaderKeyword                          shaderKeyword      { get; init; } = new();
        public QuotedStringLiteral                    name               { get; init; }
        public OpenBraceToken                         openBraceToken     { get; init; } = new();
        public MaterialProperties?                    materialProperties { get; init; }
        public SyntaxList<shaderlab, ShaderStatement> shaderStatements   { get; init; } = new();
        public CloseBraceToken                        closeBraceToken    { get; init; } = new();
    }
}
