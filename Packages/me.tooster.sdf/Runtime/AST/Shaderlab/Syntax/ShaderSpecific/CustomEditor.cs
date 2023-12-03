namespace me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific {
    // CustomEditor "editor"
    [SyntaxNode] public partial record CustomEditor : ShaderStatement {
        public CustomEditorKeyword customEditorKeyword { get; init; } = new();
        public QuotedStringLiteral editor              { get; init; }
    }
}
