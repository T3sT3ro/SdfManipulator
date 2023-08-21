using System.Collections.Generic;

namespace AST.Hlsl.Syntax.Expressions.Operators {
    public abstract record Affix : Expression { // prefix/postfix increment/decrement
        public Identifier id { get; set; }

        // --x
        // ++x
        public record Pre : Affix {
            public AffixOperatorToken prefixOperator { get; set; }

            public override IReadOnlyList<IHlslSyntaxOrToken> ChildNodesAndTokens =>
                new IHlslSyntaxOrToken[] { prefixOperator, id };
        }

        // x--
        // x++
        public record Post : Affix {
            public AffixOperatorToken suffixOperator { get; set; }

            public override IReadOnlyList<IHlslSyntaxOrToken> ChildNodesAndTokens =>
                new IHlslSyntaxOrToken[] { id, suffixOperator };
        }
    }
}
