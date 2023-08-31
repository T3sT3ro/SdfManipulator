#nullable enable
using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Statements {
    public partial record If {
        public partial record ElseClause : Syntax<Hlsl> {
            private readonly ElseKeyword _elseKeyword;
            private readonly Statement   _statement;

            public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
                { elseKeyword, statement };
        }
    }
}
