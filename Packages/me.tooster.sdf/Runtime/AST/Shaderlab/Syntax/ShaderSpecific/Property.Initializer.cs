namespace me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific {
    public partial record Property {
        // = ...
        [SyntaxNode] public abstract partial record Initializer {
            public EqualsToken equalsToken { get; init; } = new();
        }

        // = (1, 2, 3, 4)
        [SyntaxNode] public partial record Vector : Initializer {
            public ArgumentList<LiteralExpression<NumberLiteral>> arguments { get; init; } = new();
        }

        // = "red" {}
        [SyntaxNode] public partial record Texture : Initializer {
            public QuotedStringLiteral textureName { get; init; } = new();
            public OpenBraceToken      openBrace   { get; init; } = new();
            public CloseBraceToken     closeBrace  { get; init; } = new();
        }

        // = 1 
        [SyntaxNode] public partial record Number<T> : Initializer where T : NumberLiteral {
            public T numberLiteral { get; init; }
        }
    }
}
