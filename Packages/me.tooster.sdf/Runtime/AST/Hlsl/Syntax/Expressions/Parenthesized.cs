using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Expressions {
    [SyntaxNode] public partial record Parenthesized : Expression {
        public OpenParenToken  openParen  { get; init; } = new();
        public Expression      expression { get; init; }
        public CloseParenToken closeParen { get; init; } = new();
    }
}
