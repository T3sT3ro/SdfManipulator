using System.Collections.Generic;

namespace AST.Shaderlab.Syntax.Shader {
    public partial record Property {
        public abstract record Initializer : ShaderlabSyntax {
            public EqualsToken equalsToken { get; set; } = new();
        }
    }

    public partial record Property {
        public record Vector : Initializer {
            public ArgumentList<LiteralExpression<NumberLiteral>> arguments { get; set; }

            public override IReadOnlyList<IShaderlabSyntaxOrToken> ChildNodesAndTokens => new IShaderlabSyntaxOrToken[]
                { equalsToken, arguments };
        }
    }

    public partial record Property {
        public record Texture : Initializer {
            public QuotedStringLiteral textureName { get; set; }
            public OpenBraceToken      openBrace   { get; set; } = new();
            public CloseBraceToken     closeBrace  { get; set; } = new();

            public override IReadOnlyList<IShaderlabSyntaxOrToken> ChildNodesAndTokens => new IShaderlabSyntaxOrToken[]
                { equalsToken, textureName, openBrace, closeBrace };
        }
    }

    public partial record Property {
        public record Number<T> : Initializer where T : NumberLiteral {
            public T numberLiteral { get; set; }

            public override IReadOnlyList<IShaderlabSyntaxOrToken> ChildNodesAndTokens => new ShaderlabToken[]
                { equalsToken, numberLiteral };
        }
    }
}
