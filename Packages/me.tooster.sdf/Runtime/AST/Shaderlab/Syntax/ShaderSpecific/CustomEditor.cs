using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific {
    // CustomEditor "editor"
    [Syntax] public partial record CustomEditor : ShaderStatement {
        [Init] private readonly CustomEditorKeyword _customEditorKeyword;
        private readonly        QuotedStringLiteral _editor;
    }
}
