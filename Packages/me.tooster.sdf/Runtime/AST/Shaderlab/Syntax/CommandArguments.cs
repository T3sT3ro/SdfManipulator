using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax {
    /// fixed argument: Blend Off
    /// calculated argument: Blend [_BlendState]
    [SyntaxNode] public abstract partial record CommandArgument {
        public static implicit operator CommandArgument(ArgumentKeyword arg) => (PredefinedArgument)arg;
    }

    /// <summary>Command argument using material property e.g. <c>Cull [_CullValue]</c></summary>
    /// <remarks><a href="https://docs.unity3d.com/Manual/SL-Properties.html">Unity docs</a></remarks>
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
