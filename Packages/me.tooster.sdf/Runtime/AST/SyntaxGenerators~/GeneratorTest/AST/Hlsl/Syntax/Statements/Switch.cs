#nullable enable
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    [AstSyntax] public partial record Switch : Statement {
        public SwitchKeyword          switchKeyword { get; init; }
        public OpenParenToken         openParen { get; init; }
        public Identifier             selector { get; init; }
        public CloseParenToken        closeParen { get; init; }
        public SyntaxList<Hlsl, Case> cases { get; init; }
        public           DefaultCase?           @default { get; init; }
    }
}
