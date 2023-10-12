using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators {
    [AstSyntax] public partial record Binary : Expression {
        public Expression  left { get; init; }
        public Token<Hlsl> operatorToken { get; init; }
        public Expression  right { get; init; }
    }
}
