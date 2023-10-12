using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    // = y
    // = {{a}, {b}}}
    [AstSyntax] public abstract partial record Initializer : Syntax<Hlsl> {
        public EqualsToken equalsToken { get; init; } = new();
        public        Expression  value { get; init; }
    }
}
