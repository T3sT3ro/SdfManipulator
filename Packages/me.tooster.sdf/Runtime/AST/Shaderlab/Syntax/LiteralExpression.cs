using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax {
    [Syntax] public partial record  LiteralExpression<T> : Syntax<Shaderlab> where T : Literal {
        private readonly  Literal _literal ;
    }
}
