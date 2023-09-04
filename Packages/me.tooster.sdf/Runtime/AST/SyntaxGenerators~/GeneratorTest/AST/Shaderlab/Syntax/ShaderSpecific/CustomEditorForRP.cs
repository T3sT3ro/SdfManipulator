using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific {
    // CustomEditorForRenderPipeline "editor" "pipeline"
    // custom editor for render pipeline
    [Syntax] public partial record CustomEditorForRP : ShaderStatement {
        [Init] private readonly CustomEditorForRenderPipelineKeyword _customEditorForRenderPipelineKeyword;
        private readonly        QuotedStringLiteral                  _editor;
        private readonly        QuotedStringLiteral                  _pipeline;
    }
}
