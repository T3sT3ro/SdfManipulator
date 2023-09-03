using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Expressions {
    // {EXPR, EPXR, ...}
    [Syntax] public partial record StructInitializer : Expression {
        [Init] private readonly BracedList<Expression> _components;
    }
}
