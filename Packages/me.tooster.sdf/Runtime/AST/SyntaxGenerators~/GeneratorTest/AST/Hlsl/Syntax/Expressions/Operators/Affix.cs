using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators {
    [AstSyntax] public abstract partial record Affix : Expression { // prefix/postfix increment/decrement
        public Identifier id { get; init; }

        // ++x, --x
        [AstSyntax] public partial record Pre : Affix {
            public AffixOperatorToken prefixOperator { get; init; }

            public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            {
                _prefixOperator,
                _id
            };
        }

        // x++, x--
        [AstSyntax] public partial record Post : Affix {
            public AffixOperatorToken postfixOperator { get; init; }
        }
    }
}
