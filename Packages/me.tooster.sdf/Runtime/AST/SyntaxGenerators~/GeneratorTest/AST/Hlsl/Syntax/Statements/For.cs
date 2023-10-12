#nullable enable
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    // for (start conditions; conditions; increments) { body }
    // start conditions:
    //    declarations: int i, j = 0;
    //    initializers: i = 0, j = 0;
    [AstSyntax] public partial record For : Statement {
        public ForKeyword                       forKeyword { get; init; } = new();
        public OpenParenToken                   openParen { get; init; } = new();
        public        Initializer?                     initializer { get; init; }
        public SemicolonToken                   firstSemicolonToken { get; init; } = new();
        public        Expression?                      condition { get; init; }
        public SemicolonToken                   secondSemicolonToken { get; init; } = new();
        public SeparatedList<Hlsl, Expression>? increments { get; init; } = new();
        public CloseParenToken                  closeParen { get; init; } = new();
        public        Statement                        body { get; init; }

        [AstSyntax] public abstract partial record Initializer : Syntax<Hlsl>;
    }
}
