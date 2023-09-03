using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax.Commands {
    public record ZClip : Command {
        public ZClipKeyword    _zClipKeyword { get; init; } = new();
        public CommandArgument _enabled      { get; init; }

        public override IReadOnlyList<SyntaxOrToken<Shaderlab>> ChildNodesAndTokens => new SyntaxOrToken<Shaderlab>[]
            { zClipKeyword, enabled };
    }
}
