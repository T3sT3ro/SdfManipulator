using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax.Commands {
    public record BlendOp : Command {
        public BlendOp         _blendOpKeyword { get; init; } = new();
        public CommandArgument _operationArg   { get; init; }

        public override IReadOnlyList<SyntaxOrToken<Shaderlab>> ChildNodesAndTokens => new SyntaxOrToken<Shaderlab>[]
            { blendOpKeyword, operationArg };
    }
}
