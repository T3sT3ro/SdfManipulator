using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax {
    // fixed argument: Blend Off
    // calculated argument: Blend [_BlendState]
    [Syntax] public abstract partial record CommandArgument : Syntax<Shaderlab>;

    /// Undocumented feature of syntax that uses material property e.g.: Cull [_CullValue]
    [Syntax] public partial record CalculatedArgument : CommandArgument {
        [Init] private readonly OpenBracketToken  _openBracket;
        private readonly        IdentifierToken   _id;
        [Init] private readonly CloseBracketToken _closeBracket;
    }

    /// Uses a keyword like "Off", "SrcAlpha" etc.
    [Syntax] public partial record PredefinedArgument : CommandArgument {
        private readonly Token<Shaderlab> _value;
    }
}
