#nullable enable
using System.Collections.Generic;
using System.Linq;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Statements {
    public partial record Switch {
        public record Case : Syntax<Hlsl> {
            public CaseKeyword                 caseKeyword { get; init; } = new();
            public IntLiteral                  label       { get; init; }
            public ColonToken                  colonToken  { get; init; } = new();
            public SyntaxList<Hlsl, Statement> body        { get; init; } = SyntaxList<Hlsl, Statement>.Empty;

            public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
                { caseKeyword, label, colonToken, body }.ToList();
        }
    }
}
