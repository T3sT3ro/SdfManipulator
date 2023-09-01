#nullable enable
using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    public partial record Switch {
        public partial record DefaultCase : Syntax<Hlsl> {
            private readonly DefaultKeyword              /*_*/defaultKeyword;
            private readonly ColonToken                  /*_*/colonToken;
            private readonly SyntaxList<Hlsl, Statement> /*_*/body;

            public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
                { defaultKeyword, colonToken, body };
        }
    }
}
