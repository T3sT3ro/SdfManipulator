using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators {
    [AstSyntax] public partial record Ternary : Expression {
        public        Expression    condition { get; init; }
        public QuestionToken questionToken { get; init; } = new();
        public        Expression    whenTrue { get; init; }
        public ColonToken    colonToken { get; init; } = new();
        public        Expression    whenFalse { get; init; }
    }
}
