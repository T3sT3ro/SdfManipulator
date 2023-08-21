using System.Collections.Generic;

namespace AST.Shaderlab.Syntax.Commands {
    public record ZWrite : Command {
        public ZWriteKeyword   zWriteKeyword { get; set; } = new();
        public CommandArgument state         { get; set; }

        public override IReadOnlyList<IShaderlabSyntaxOrToken> ChildNodesAndTokens => new IShaderlabSyntaxOrToken[]
            { zWriteKeyword, state };
    }
}
