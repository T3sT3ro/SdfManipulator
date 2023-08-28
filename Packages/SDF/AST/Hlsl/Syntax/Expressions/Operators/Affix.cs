using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Expressions.Operators {
    public abstract record Affix : Expression { // prefix/postfix increment/decrement
        public Identifier id { get; init; }

        // ++x, --x
        public record Pre : Affix {
            public AffixOperatorToken prefixOperator { get; init; }

            public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
                { prefixOperator, id };
        }

        // x++, x--
        public record Post : Affix {
            public AffixOperatorToken suffixOperator { get; init; }

            public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
                { id, suffixOperator };
        }
    }
}
