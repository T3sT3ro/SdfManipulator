using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    [AstSyntax] public partial record Typedef : Statement {
        public TypedefKeyword typedefKeyword { get; init; } = new();
        public        Type           type { get; init; }
        public        Identifier     id { get; init; }
    }
}
