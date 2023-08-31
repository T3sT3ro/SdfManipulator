using System.Collections.Generic;
using AST.Syntax;

namespace AST.Shaderlab.Syntax.ShaderSpecific {
    public partial record Property {
        // = ...
        public abstract record Initializer : Syntax<Shaderlab> {
            public EqualsToken equalsToken { get; init; } = new();
        }
    }

    public partial record Property {
        // = (1, 2, 3, 4)
        public record Vector : Initializer {
            public ArgumentList<LiteralExpression<NumberLiteral>> arguments { get; init; }

            public override IReadOnlyList<SyntaxOrToken<Shaderlab>> ChildNodesAndTokens => new SyntaxOrToken<Shaderlab>[]
                { equalsToken, arguments };
        }
    }

    public partial record Property {
        // = "red" {}
        public record Texture : Initializer {
            public QuotedStringLiteral textureName { get; init; }
            public OpenBraceToken      openBrace   { get; init; } = new();
            public CloseBraceToken     closeBrace  { get; init; } = new();

            public override IReadOnlyList<SyntaxOrToken<Shaderlab>> ChildNodesAndTokens => new SyntaxOrToken<Shaderlab>[]
                { equalsToken, textureName, openBrace, closeBrace };
        }
    }

    public partial record Property {
        // = 1 
        public record Number<T> : Initializer where T : NumberLiteral {
            public T numberLiteral { get; init; }

            public override IReadOnlyList<SyntaxOrToken<Shaderlab>> ChildNodesAndTokens => new Token<Shaderlab>[]
                { equalsToken, numberLiteral };
        }
    }
}
