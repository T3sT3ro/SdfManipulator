using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific {
    // CustomEditor "editor"
    public record CustomEditor : ShaderStatement {
        public CustomEditorKeyword customEditorKeyword { get; init; } = new();
        public QuotedStringLiteral editor              { get; init; }

        public override IReadOnlyList<SyntaxOrToken<Shaderlab>> ChildNodesAndTokens => new Token<Shaderlab>[] 
            { customEditorKeyword, editor };
    }
}
