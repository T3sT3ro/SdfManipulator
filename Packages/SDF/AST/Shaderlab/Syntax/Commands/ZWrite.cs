using System.Collections.Generic;
using AST.Syntax;

namespace AST.Shaderlab.Syntax.Commands {
    public record ZWrite : Command {
        public ZWriteKeyword   zWriteKeyword { get; init; } = new();
        public CommandArgument state         { get; init; }

        public override IReadOnlyList<SyntaxOrToken<Shaderlab>> ChildNodesAndTokens => new SyntaxOrToken<Shaderlab>[]
            { zWriteKeyword, state };
    }
}
