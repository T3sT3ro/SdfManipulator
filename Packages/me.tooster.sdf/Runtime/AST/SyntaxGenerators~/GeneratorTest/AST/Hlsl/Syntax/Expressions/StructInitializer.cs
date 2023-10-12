using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Expressions {
    // {EXPR, EPXR, ...}
    [AstSyntax] public partial record StructInitializer : Expression {
        public BracedList<Expression> components { get; init; } = new();
    }
}
