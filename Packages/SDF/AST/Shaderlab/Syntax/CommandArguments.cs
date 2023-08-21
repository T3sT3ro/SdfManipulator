using System.Collections.Generic;

namespace AST.Shaderlab.Syntax {
    public abstract record CommandArgument : ShaderlabSyntax;

    // undocumented feature of syntax along these lines:
    // Cull [_CullValue]
    public record CalculatedArgument : CommandArgument {
        public OpenBracketToken  openBracket  { get; set; } = new();
        public IdentifierToken   id           { get; set; }
        public CloseBracketToken closeBracket { get; set; } = new();

        public override IReadOnlyList<IShaderlabSyntaxOrToken> ChildNodesAndTokens => new IShaderlabSyntaxOrToken[]
            { openBracket, id, closeBracket };
    }

    // Uses a keyword like "Off", "SrcAlpha" etc.
    public record PredefinedArgument : CommandArgument {
        public ShaderlabToken value { get; set; }

        public override IReadOnlyList<IShaderlabSyntaxOrToken> ChildNodesAndTokens => new[] { value };
    }
}
