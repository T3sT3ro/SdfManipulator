using System.Collections.Generic;
using System.Text;
using AST.Hlsl;
using AST.Hlsl.Syntax;

namespace AST.Shaderlab.Syntax {
    public record HlslInclude : SubShaderOrPassStatement {
        public HlslIncludeKeyword hlslIncludeKeyword { get; init; } = new();
        public HlslTree           hlslTree           { get; init; }
        public EndHlslKeyword     endHlslKeyword     { get; init; } = new();

        public override IReadOnlyList<IShaderlabSyntaxOrToken> ChildNodesAndTokens => new ShaderlabToken[]
            { hlslIncludeKeyword, endHlslKeyword };

        public override void WriteTo(StringBuilder sb) {
            sb.Append(hlslIncludeKeyword.Text);
            hlslTree.Root.WriteTo(sb);
            sb.Append(endHlslKeyword.Text);
        }
    }

    public record HlslProgram : PassStatement {
        public HlslProgramKeyword hlslProgramKeyword { get; init; } = new();
        public HlslTree           hlslTree           { get; init; }
        public EndHlslKeyword     endHlslKeyword     { get; init; } = new();

        public override IReadOnlyList<IShaderlabSyntaxOrToken> ChildNodesAndTokens => new ShaderlabToken[]
            { hlslProgramKeyword, endHlslKeyword };

        public override void WriteTo(StringBuilder sb) {
            sb.Append(hlslProgramKeyword.Text);
            hlslTree.Root.WriteTo(sb);
            sb.Append(endHlslKeyword.Text);
        }
    }
}
