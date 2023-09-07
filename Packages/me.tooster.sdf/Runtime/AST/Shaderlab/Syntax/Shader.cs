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
    [Syntax] public partial record Shader : Syntax<Shaderlab> {
        [Init] private readonly ShaderKeyword                          _shaderKeyword;
        private readonly        QuotedStringLiteral                    _name;
        [Init] private readonly OpenBraceToken                         _openBraceToken;
        private readonly        MaterialProperties?                    _materialProperties;
        [Init] private readonly SyntaxList<Shaderlab, ShaderStatement> _shaderStatements;
        [Init] private readonly CloseBraceToken                        _closeBraceToken;
    }
}
