using System.Collections.Generic;
using AST.Syntax;

namespace AST.Shaderlab.Syntax.ShaderSpecific {
    // CustomEditorForRenderPipeline "editor" "pipeline"
    // custom editor for render pipeline
    public record CustomEditorForRP : ShaderStatement {
        public CustomEditorForRenderPipelineKeyword customEditorForRenderPipelineKeyword { get; init; } = new();
        public QuotedStringLiteral                  editor                               { get; init; }
        public QuotedStringLiteral                  pipeline                             { get; init; }

        public override IReadOnlyList<SyntaxOrToken<Shaderlab>> ChildNodesAndTokens => new Token<Shaderlab>[]
            { customEditorForRenderPipelineKeyword, editor, pipeline };
    }
}
