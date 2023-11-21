using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators {
    [SyntaxNode] public partial record Binary : Expression {
        public Expression  left          { get; init; }
        public Token<hlsl> operatorToken { get; init; }
        public Expression  right         { get; init; }
    }
}
