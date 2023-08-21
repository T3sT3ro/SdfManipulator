#nullable enable
using System.Collections.Generic;
using System.Linq;
using AST.Syntax;

namespace AST.Shaderlab.Syntax.Shader {
    /// <a href="https://docs.unity3d.com/Manual/SL-Shader.html">Shader</a>
    /// <a href="https://docs.unity3d.com/Manual/SL-Fallback.html">Fallback</a>
    /// <a href="https://docs.unity3d.com/Manual/SL-CustomEditor.html">CustomEditor</a>
    public record Shader : ShaderlabSyntax {
        public ShaderKeyword                  shaderKeyword      { get; set; } = new();
        public QuotedStringLiteral            name               { get; set; }
        public OpenBraceToken                 openBraceToken     { get; set; } = new();
        public MaterialProperties?            materialProperties { get; set; }
        public IReadOnlyList<ShaderStatement> shaderStatements   { get; set; }
        public CloseBraceToken                closeBraceToken    { get; set; } = new();

        public override IReadOnlyList<IShaderlabSyntaxOrToken> ChildNodesAndTokens => new IShaderlabSyntaxOrToken[]
                { shaderKeyword, name, openBraceToken, materialProperties }
            .Concat(shaderStatements)
            .Append(closeBraceToken)
            .FilterNotNull().ToList();
    }
}
