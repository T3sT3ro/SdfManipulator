#nullable enable
using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax.Commands {
    public record Cull : Command {
        public CullKeyword     cullKeyword { get; init; } = new();
        public CommandArgument state       { get; init; }

        public override IReadOnlyList<SyntaxOrToken<Shaderlab>> ChildNodesAndTokens => new SyntaxOrToken<Shaderlab>[]
            { cullKeyword, state };
    }
}
