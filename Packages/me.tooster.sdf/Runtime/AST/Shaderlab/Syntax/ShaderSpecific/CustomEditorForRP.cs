using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific {
    // CustomEditorForRenderPipeline "editor" "pipeline"
    // custom editor for render pipeline
    [SyntaxNode] public partial record CustomEditorForRP : ShaderStatement {
        public CustomEditorForRenderPipelineKeyword customEditorForRenderPipelineKeyword { get; init; } = new();
        public QuotedStringLiteral                  editor                               { get; init; }
        public QuotedStringLiteral                  pipeline                             { get; init; }
    }
}
