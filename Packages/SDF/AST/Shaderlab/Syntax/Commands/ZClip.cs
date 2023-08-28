using System.Collections.Generic;
using AST.Syntax;

namespace AST.Shaderlab.Syntax.Commands {
    public record ZClip : Command {
        public ZClipKeyword    zClipKeyword { get; init; } = new();
        public CommandArgument enabled      { get; init; }

        public override IReadOnlyList<SyntaxOrToken<Shaderlab>> ChildNodesAndTokens => new SyntaxOrToken<Shaderlab>[]
            { zClipKeyword, enabled };
    }
}
