using System.Collections.Generic;

namespace AST.Shaderlab.Syntax.Shader {
    // custom editor for render pipeline
    public record CustomEditorForRP : ShaderStatement {
        public CustomEditorForRenderPipelineKeyword customEditorForRenderPipelineKeyword { get; set; } = new();
        public QuotedStringLiteral                  editor                               { get; set; }
        public QuotedStringLiteral                  pipeline                             { get; set; }

        public override IReadOnlyList<IShaderlabSyntaxOrToken> ChildNodesAndTokens => new ShaderlabToken[]
            { customEditorForRenderPipelineKeyword, editor, pipeline };
    }
}
