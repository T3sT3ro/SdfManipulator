using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators {
    // a, b
    [AstSyntax] public partial record Comma : Expression {
        public        Expression left { get; init; }
        public CommaToken comma { get; init; } = new();
        public        Expression right { get; init; }
    }
}
