using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax {
    // fixed argument: Blend Off
    // calculated argument: Blend [_BlendState]
    [AstSyntax] public abstract partial record CommandArgument : Syntax<Shaderlab>;

    /// Undocumented feature of syntax that uses material property e.g.: Cull [_CullValue]
    [AstSyntax] public partial record CalculatedArgument : CommandArgument {
        public OpenBracketToken  openBracket { get; init; } = new();
        public        IdentifierToken   id { get; init; }
        public CloseBracketToken closeBracket { get; init; } = new();
    }

    /// Uses a keyword like "Off", "SrcAlpha" etc.
    [AstSyntax] public partial record PredefinedArgument : CommandArgument {
        public Token<Shaderlab> value { get; init; }
    }
}
