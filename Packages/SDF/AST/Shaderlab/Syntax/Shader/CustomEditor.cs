using System.Collections.Generic;

namespace AST.Shaderlab.Syntax.Shader {
    public record CustomEditor : ShaderStatement {
        public CustomEditorKeyword customEditorKeyword { get; set; } = new();
        public QuotedStringLiteral editor              { get; set; }

        public override IReadOnlyList<IShaderlabSyntaxOrToken> ChildNodesAndTokens => new ShaderlabToken[] 
            { customEditorKeyword, editor };
    }
}
