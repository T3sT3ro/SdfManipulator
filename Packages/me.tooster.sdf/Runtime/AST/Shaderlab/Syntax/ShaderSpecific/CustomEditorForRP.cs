using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific {
    // CustomEditorForRenderPipeline "editor" "pipeline"
    // custom editor for render pipeline
    [Syntax] public partial recordCustomEditorForRP : ShaderStatement {
        public CustomEditorForRenderPipelineKeyword _customEditorForRenderPipelineKeyword { get; init; } = new();
        public QuotedStringLiteral                  _editor                               { get; init; }
        public QuotedStringLiteral                  _pipeline                             { get; init; }

        public override IReadOnlyList<SyntaxOrToken<Shaderlab>> ChildNodesAndTokens => new Token<Shaderlab>[]
            { customEditorForRenderPipelineKeyword, editor, pipeline };
    }
}
