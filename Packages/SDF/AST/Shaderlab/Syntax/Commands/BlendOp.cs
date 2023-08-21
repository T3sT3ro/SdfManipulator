using System.Collections.Generic;

namespace AST.Shaderlab.Syntax.Commands {
    public record BlendOp : Command {
        public BlendOp         blendOpKeyword  { get; set; } = new();
        public CommandArgument operationArg { get; set; }

        public override IReadOnlyList<IShaderlabSyntaxOrToken> ChildNodesAndTokens => new IShaderlabSyntaxOrToken[]
            { blendOpKeyword, operationArg };
    }
}
