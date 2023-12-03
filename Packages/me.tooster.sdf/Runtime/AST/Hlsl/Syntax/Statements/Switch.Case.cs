#nullable enable
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    public partial record Switch {
        [SyntaxNode] public partial record Case {
            public CaseKeyword                 caseKeyword { get; init; } = new();
            public IntLiteral                  label       { get; init; }
            public ColonToken                  colonToken  { get; init; } = new();
            public SyntaxList<hlsl, Statement> body        { get; init; } = new();
        }
    }
}
