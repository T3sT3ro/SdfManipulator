using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Expressions.Operators {
    public abstract partial record Affix : Expression { // prefix/postfix increment/decrement
        private readonly Identifier _id;

        // ++x, --x
        public partial record Pre : Affix {
            private readonly AffixOperatorToken _prefixOperator;

            public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
                { prefixOperator, id };
        }

        // x++, x--
        public partial record Post : Affix {
            private readonly AffixOperatorToken _suffixOperator;

            public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
                { id, suffixOperator };
        }
    }
}
