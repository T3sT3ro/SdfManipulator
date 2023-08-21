using System.Collections.Generic;
using System.Text;
using AST.Hlsl;

namespace AST.Shaderlab.Syntax {
    public record HlslInclude : SubShaderOrPassStatement {
        public HlslIncludeKeyword hlslIncludeKeyword { get; set; } = new();
        public HlslTree           hlslTree           { get; set; }
        public EndHlslKeyword     endHlslKeyword     { get; set; } = new();

        public override IReadOnlyList<IShaderlabSyntaxOrToken> ChildNodesAndTokens => new ShaderlabToken[]
            { hlslIncludeKeyword, endHlslKeyword };

        public override void WriteTo(StringBuilder sb) {
            sb.Append(hlslIncludeKeyword.Text);
            hlslTree.Root.WriteTo(sb);
            sb.Append(endHlslKeyword.Text);
        }
    }

    public record HlslProgram : PassStatement {
        public HlslProgramKeyword hlslProgramKeyword { get; set; } = new();
        public HlslTree           hlslTree           { get; set; }
        public EndHlslKeyword     endHlslKeyword     { get; set; } = new();

        public override IReadOnlyList<IShaderlabSyntaxOrToken> ChildNodesAndTokens => new ShaderlabToken[]
            { hlslProgramKeyword, endHlslKeyword };

        public override void WriteTo(StringBuilder sb) {
            sb.Append(hlslProgramKeyword.Text);
            hlslTree.Root.WriteTo(sb);
            sb.Append(endHlslKeyword.Text);
        }
    }
}
