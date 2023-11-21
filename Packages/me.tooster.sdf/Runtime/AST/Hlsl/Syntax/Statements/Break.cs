using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    [SyntaxNode] public partial record Break : Statement {
        public BreakKeyword   breakKeyword   { get; init; } = new();
        public SemicolonToken semicolonToken { get; init; } = new();
    }
}
