using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax {
    /// fixed argument: Blend Off
    /// calculated argument: Blend [_BlendState]
    /// FIXME: can command arguments arbitrary on syntax level, or are they kkeywords like "SrcAlpha"?
    [SyntaxNode] public abstract partial record CommandArgument {
        public static implicit operator CommandArgument(ArgumentKeyword arg) => (PredefinedArgument)arg;
    }

    /// Undocumented feature of syntax: command argument using material property e.g.: Cull [_CullValue]
    [SyntaxNode] public partial record CalculatedArgument : CommandArgument {
        public OpenBracketToken  openBracket  { get; init; } = new();
        public IdentifierToken   id           { get; init; }
        public CloseBracketToken closeBracket { get; init; } = new();
    }

    /// Predefined argument to shaderlab command like "Off", "SrcAlpha" etc.
    [SyntaxNode] public partial record PredefinedArgument : CommandArgument {
        public ArgumentKeyword arg { get; init; }

        public static implicit operator PredefinedArgument(ArgumentKeyword keyword) => new() { arg = keyword };
    }
}
