#nullable enable
using System.Collections.Generic;
using System.Linq;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Statements {
    public partial record Switch {
        public partial record Case : Syntax<Hlsl> {
            private readonly CaseKeyword                 _caseKeyword;
            private readonly IntLiteral                  _label;
            private readonly ColonToken                  _colonToken;
            private readonly SyntaxList<Hlsl, Statement> _body;

            public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
                { caseKeyword, label, colonToken, body }.ToList();
        }
    }
}
