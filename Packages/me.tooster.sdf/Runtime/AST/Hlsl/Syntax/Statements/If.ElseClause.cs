#nullable enable
using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    public partial record If {
        public partial record ElseClause : Syntax<Hlsl> {
            private readonly ElseKeyword /*_*/elseKeyword;
            private readonly Statement   /*_*/statement;

            public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
                { elseKeyword, statement };
        }
    }
}
