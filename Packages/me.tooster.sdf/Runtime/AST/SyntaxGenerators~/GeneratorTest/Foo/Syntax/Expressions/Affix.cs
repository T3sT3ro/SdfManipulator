using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;
using me.tooster.sdf.AST.Syntax.CommonSyntax;

namespace me.tooster.sdf.AST.Foo.Syntax.Expressions {
    public abstract partial record Affix : Expression<foo> {
        public ZeroLiteral zero { get; init; }

        [SyntaxNode] public partial record Succ : Affix {
            public PlusToken plusToken { get; init; }

            public override IReadOnlyList<SyntaxOrToken<foo>> ChildNodesAndTokens => new SyntaxOrToken<foo>[]
                { plusToken, zero };
        }

        [SyntaxNode] public partial record Pred : Affix {
            public MinusToken minusToken { get; init; }
            
            public override IReadOnlyList<SyntaxOrToken<foo>> ChildNodesAndTokens => new SyntaxOrToken<foo>[]
                { minusToken, zero };
        }
    }
}
