using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax.Commands {
    public record ZWrite : Command {
        public ZWriteKeyword   _zWriteKeyword { get; init; } = new();
        public CommandArgument _state         { get; init; }

        public override IReadOnlyList<SyntaxOrToken<Shaderlab>> ChildNodesAndTokens => new SyntaxOrToken<Shaderlab>[]
            { zWriteKeyword, state };
    }
}
