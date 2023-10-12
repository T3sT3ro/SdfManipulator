using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    [AstSyntax] public partial record Block : Statement {
        public OpenBraceToken              openBraceToken { get; init; } = new();
        public SyntaxList<Hlsl, Statement> statements { get; init; } = new();
        public CloseBraceToken             closeBraceToken { get; init; } = new();
    }
}
