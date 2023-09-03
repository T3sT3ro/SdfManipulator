using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific {
    public partial record Property {
        // = ...
        public abstract record Initializer : Syntax<Shaderlab> {
            public EqualsToken _equalsToken { get; init; } = new();

            public Initializer() : base() {}
        }
    }

    public partial record Property {
        // = (1, 2, 3, 4)
        public record Vector : Initializer {
            public ArgumentList<LiteralExpression<NumberLiteral>> _arguments { get; init; }

            public override IReadOnlyList<SyntaxOrToken<Shaderlab>> ChildNodesAndTokens => new SyntaxOrToken<Shaderlab>[]
                { equalsToken, arguments };
        }
    }

    public partial record Property {
        // = "red" {}
        public record Texture : Initializer {
            public QuotedStringLiteral _textureName { get; init; }
            public OpenBraceToken      _openBrace   { get; init; } = new();
            public CloseBraceToken     _closeBrace  { get; init; } = new();

            public override IReadOnlyList<SyntaxOrToken<Shaderlab>> ChildNodesAndTokens => new SyntaxOrToken<Shaderlab>[]
                { equalsToken, textureName, openBrace, closeBrace };
        }
    }

    public partial record Property {
        // = 1 
        public record Number<T> : Initializer where T : NumberLiteral {
            public T _numberLiteral { get; init; }

            public override IReadOnlyList<SyntaxOrToken<Shaderlab>> ChildNodesAndTokens => new Token<Shaderlab>[]
                { equalsToken, numberLiteral };
        }
    }
}
