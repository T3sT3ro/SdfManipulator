using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;
using Expression = me.tooster.sdf.AST.Syntax.CommonSyntax.Expression<me.tooster.sdf.AST.hlsl>;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators {
    [SyntaxNode] public abstract partial record Affix : Expression { // prefix/postfix increment/decrement
        public Identifier id { get; init; }

        // ++x, --x
        [SyntaxNode] public partial record Pre : Affix {
            public AffixOperatorToken prefixOperator { get; init; }

            public override IReadOnlyList<SyntaxOrToken<hlsl>> ChildNodesAndTokens() => new SyntaxOrToken<hlsl>[]
                { prefixOperator, id };
        }

        // x++, x--
        [SyntaxNode] public partial record Post : Affix {
            public AffixOperatorToken postfixOperator { get; init; }
        }
    }
}
