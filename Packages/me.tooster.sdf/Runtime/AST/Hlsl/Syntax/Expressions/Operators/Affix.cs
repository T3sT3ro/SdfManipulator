using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators {
    public abstract partial record Affix : Expression { // prefix/postfix increment/decrement
        private readonly Identifier /*_*/id;

        // ++x, --x
        public partial record Pre : Affix {
            private readonly AffixOperatorToken /*_*/prefixOperator;

            public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
                { prefixOperator, id };
        }

        // x++, x--
        public partial record Post : Affix {
            private readonly AffixOperatorToken /*_*/suffixOperator;

            public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
                { id, suffixOperator };
        }
    }
}
