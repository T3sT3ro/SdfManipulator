using System.Collections.Generic;

namespace AST.Hlsl.Syntax.Expressions.Operators {
    public abstract record Affix : Expression { // prefix/postfix increment/decrement
        public IdentifierName id { get; internal set; }

        public override IReadOnlyList<HlslSyntax> ChildNodes => new[] { id };

        public record Pre : Affix {
            public HlslToken prefixOperator { get; internal set; }

            public override IReadOnlyList<HlslSyntaxOrToken> ChildNodesAndTokens =>
                new HlslSyntaxOrToken[] { prefixOperator, id };
        };

        public record Post : Affix {
            public HlslToken suffixOperator { get; internal set; }

            public override IReadOnlyList<HlslSyntaxOrToken> ChildNodesAndTokens =>
                new HlslSyntaxOrToken[] { id, suffixOperator };
        }
    }
}
