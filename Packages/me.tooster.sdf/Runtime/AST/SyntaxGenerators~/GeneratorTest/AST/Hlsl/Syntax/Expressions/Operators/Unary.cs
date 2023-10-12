using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators {
    [AstSyntax] public partial record Unary : Expression {
        public Token<Hlsl> operatorToken { get; init; }
        public Expression  expression { get; init; }
    }
}
