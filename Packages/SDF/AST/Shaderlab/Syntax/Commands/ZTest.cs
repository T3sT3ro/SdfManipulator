using System.Collections.Generic;
using AST.Syntax;

namespace AST.Shaderlab.Syntax.Commands {
    public record ZTest : Command {
        public ZTestKeyword    zTestKeyword { get; init; } = new();
        public CommandArgument operation    { get; init; }

        public override IReadOnlyList<SyntaxOrToken<Shaderlab>> ChildNodesAndTokens => new SyntaxOrToken<Shaderlab>[]
            { zTestKeyword, operation };
    }
}
