using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators {
    // someFunction(arguments) 
    [AstSyntax] public partial record Call : Expression {
        public Identifier                 id { get; init; }
        public ArgumentList<Syntax<Hlsl>> argList { get; init; }
    }
}
