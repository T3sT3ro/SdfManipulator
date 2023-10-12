using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators {
    // a.b.c
    [AstSyntax] public partial record Member : Expression {
        public        Expression expression { get; init; }
        public DotToken   dotToken { get; init; } = new();
        public        Identifier member { get; init; }
    }
}
