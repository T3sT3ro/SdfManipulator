using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax {
    // fixed argument: Blend Off
    // calculated argument: Blend [_BlendState]
    public abstract record CommandArgument : Syntax<Shaderlab>;

    /// Undocumented feature of syntax that uses material property e.g.: Cull [_CullValue]
    public record CalculatedArgument : CommandArgument {
        public OpenBracketToken  openBracket  { get; init; } = new();
        public IdentifierToken   id           { get; init; }
        public CloseBracketToken closeBracket { get; init; } = new();

        public override IReadOnlyList<SyntaxOrToken<Shaderlab>> ChildNodesAndTokens => new SyntaxOrToken<Shaderlab>[]
            { openBracket, id, closeBracket };
    }

    /// Uses a keyword like "Off", "SrcAlpha" etc.
    public record PredefinedArgument : CommandArgument {
        public Token<Shaderlab> value { get; init; }

        public override IReadOnlyList<SyntaxOrToken<Shaderlab>> ChildNodesAndTokens => new[] 
            { value };
    }
}
