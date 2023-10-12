using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific {
    public partial record Property {
        // = ...
        [AstSyntax] public abstract partial record Initializer : Syntax<Shaderlab> {
            public EqualsToken equalsToken { get; init; } = new();
        }

        // = (1, 2, 3, 4)
        [AstSyntax] public partial record Vector : Initializer {
            public ArgumentList<LiteralExpression<NumberLiteral>> arguments { get; init; } = new();
        }

        // = "red" {}
        [AstSyntax] public partial record Texture : Initializer {
            public QuotedStringLiteral textureName { get; init; } = new();
            public OpenBraceToken      openBrace { get; init; } = new();
            public CloseBraceToken     closeBrace { get; init; } = new();
        }

        // = 1 
        [AstSyntax] public partial record Number<T> : Initializer where T : NumberLiteral {
            public T numberLiteral { get; init; }
        }
    }
}
