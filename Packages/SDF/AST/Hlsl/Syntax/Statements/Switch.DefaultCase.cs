#nullable enable
using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Statements {
    public partial record Switch {
        public partial record DefaultCase : Syntax<Hlsl> {
            private readonly DefaultKeyword              _defaultKeyword;
            private readonly ColonToken                  _colonToken;
            private readonly SyntaxList<Hlsl, Statement> _body;

            public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
                { defaultKeyword, colonToken, body };
        }
    }
}
