using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific {
    public partial record Property {
        // = ...
        [Syntax] public abstract partial record Initializer : Syntax<Shaderlab> {
            [Init] private readonly EqualsToken _equalsToken;
        }

        // = (1, 2, 3, 4)
        [Syntax] public partial record Vector : Initializer {
            [Init] private readonly ArgumentList<LiteralExpression<NumberLiteral>> _arguments;

        }

        // = "red" {}
        [Syntax] public partial record Texture : Initializer {
            [Init] private readonly QuotedStringLiteral _textureName;
            [Init] private readonly OpenBraceToken      _openBrace;
            [Init] private readonly CloseBraceToken     _closeBrace;
        }

        // = 1 
        [Syntax] public partial record Number<T> : Initializer where T : NumberLiteral {
            private readonly T _numberLiteral;
        }
    }
}
