using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    [AstSyntax] public partial record Discard : Statement {
        public DiscardKeyword discardKeyword { get; init; } = new();
        public SemicolonToken semicolonToken { get; init; } = new();
    }
}
