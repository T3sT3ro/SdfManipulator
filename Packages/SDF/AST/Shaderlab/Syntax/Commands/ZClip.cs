using System.Collections.Generic;
using AST.Shaderlab.Syntax.SubShader;

namespace AST.Shaderlab.Syntax.Commands {
    public record ZClip : Command {
        public ZClipKeyword    zClipKeyword { get; set; } = new();
        public CommandArgument enabled      { get; set; }

        public override IReadOnlyList<IShaderlabSyntaxOrToken> ChildNodesAndTokens => new IShaderlabSyntaxOrToken[]
            { zClipKeyword, enabled };
    }
}
