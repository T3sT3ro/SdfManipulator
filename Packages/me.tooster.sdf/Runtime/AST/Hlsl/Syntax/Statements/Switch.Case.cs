#nullable enable
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    public partial record Switch {
        public partial record Case : Syntax<Hlsl> {
            private readonly CaseKeyword                 /*_*/caseKeyword;
            private readonly IntLiteral                  /*_*/label;
            private readonly ColonToken                  /*_*/colonToken;
            private readonly SyntaxList<Hlsl, Statement> /*_*/body;

            public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
                { caseKeyword, label, colonToken, body }.ToList();
        }
    }
}
