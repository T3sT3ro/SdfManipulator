using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax.Commands {
    public record ZTest : Command {
        public ZTestKeyword    _zTestKeyword { get; init; } = new();
        public CommandArgument _operation    { get; init; }

        public override IReadOnlyList<SyntaxOrToken<Shaderlab>> ChildNodesAndTokens => new SyntaxOrToken<Shaderlab>[]
            { zTestKeyword, operation };
    }
}
