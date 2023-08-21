using System.Collections.Generic;
using AST.Shaderlab.Syntax.SubShader;

namespace AST.Shaderlab.Syntax.Commands {
    public record ZTest : Command {
        public ZTestKeyword    zTestKeyword { get; set; } = new();
        public CommandArgument operation    { get; set; }

        public override IReadOnlyList<IShaderlabSyntaxOrToken> ChildNodesAndTokens => new IShaderlabSyntaxOrToken[]
            { zTestKeyword, operation };
    }
}
