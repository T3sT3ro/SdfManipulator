using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Expressions {
    [AstSyntax] public partial record LiteralExpression<T> : Expression where T : Literal {
        public T literal { get; init; }
    }
}
