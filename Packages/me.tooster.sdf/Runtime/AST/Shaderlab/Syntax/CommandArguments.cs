using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax {
    // fixed argument: Blend Off
    // calculated argument: Blend [_BlendState]
    [SyntaxNode] public abstract partial record CommandArgument;

    /// Undocumented feature of syntax that uses material property e.g.: Cull [_CullValue]
    [SyntaxNode] public partial record CalculatedArgument : CommandArgument {
        public OpenBracketToken  openBracket  { get; init; } = new();
        public IdentifierToken   id           { get; init; }
        public CloseBracketToken closeBracket { get; init; } = new();
    }

    /// Uses a keyword like "Off", "SrcAlpha" etc.
    [SyntaxNode] public partial record PredefinedArgument : CommandArgument {
        public Token<shaderlab> value { get; init; }
    }
}
