using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax {
    [AstSyntax] public partial record LiteralExpression<T> : Syntax<Shaderlab> where T : Literal {
        public Literal literal { get; init; }
    }
}
