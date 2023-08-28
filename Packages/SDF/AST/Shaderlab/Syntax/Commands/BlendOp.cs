using System.Collections.Generic;
using AST.Syntax;

namespace AST.Shaderlab.Syntax.Commands {
    public record BlendOp : Command {
        public BlendOp         blendOpKeyword  { get; init; } = new();
        public CommandArgument operationArg { get; init; }

        public override IReadOnlyList<SyntaxOrToken<Shaderlab>> ChildNodesAndTokens => new SyntaxOrToken<Shaderlab>[]
            { blendOpKeyword, operationArg };
    }
}
