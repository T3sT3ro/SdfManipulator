#nullable enable
using System.Collections.Generic;

namespace AST.Shaderlab.Syntax.Commands {
    public record Cull : Command {
        public CullKeyword     cullKeyword { get; set; } = new();
        public CommandArgument state       { get; set; }

        public override IReadOnlyList<IShaderlabSyntaxOrToken> ChildNodesAndTokens => new IShaderlabSyntaxOrToken[]
            { cullKeyword, state };
    }
}
