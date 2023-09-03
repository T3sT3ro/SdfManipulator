using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific {
    // CustomEditor "editor"
    public record CustomEditor : ShaderStatement {
        public CustomEditorKeyword _customEditorKeyword { get; init; } = new();
        public QuotedStringLiteral _editor              { get; init; }

        public override IReadOnlyList<SyntaxOrToken<Shaderlab>> ChildNodesAndTokens => new Token<Shaderlab>[] 
            { customEditorKeyword, editor };
    }
}
