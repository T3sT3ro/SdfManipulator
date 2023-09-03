using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators {
    [Syntax] public abstract partial record Affix : Expression { // prefix/postfix increment/decrement
        private readonly Identifier _id;

        // ++x, --x
        [Syntax] public partial record Pre : Affix {
            private readonly AffixOperatorToken _prefixOperator;

            public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
                { prefixOperator, id };
        }

        // x++, x--
        [Syntax] public partial record Post : Affix {
            private readonly AffixOperatorToken _suffixOperator;

            public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
                { id, suffixOperator };
        }
    }
}
