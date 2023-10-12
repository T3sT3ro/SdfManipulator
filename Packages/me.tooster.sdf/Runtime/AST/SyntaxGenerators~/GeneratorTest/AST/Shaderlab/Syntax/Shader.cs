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
    [AstSyntax] public partial record Shader : Syntax<Shaderlab> {
        public ShaderKeyword                          shaderKeyword { get; init; } = new();
        public        QuotedStringLiteral                    name { get; init; }
        public OpenBraceToken                         openBraceToken { get; init; } = new();
        public        MaterialProperties?                    materialProperties { get; init; }
        public SyntaxList<Shaderlab, ShaderStatement> shaderStatements { get; init; } = new();
        public CloseBraceToken                        closeBraceToken { get; init; } = new();
    }
}
