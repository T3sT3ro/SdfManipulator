#nullable enable
using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    // expression;
    // ; <- empty expression statement
    [SyntaxNode] public partial record ExpressionStatement : Statement {
        public Expression?    expression     { get; init; }
        public SemicolonToken semicolonToken { get; init; } = new();
    }
}
