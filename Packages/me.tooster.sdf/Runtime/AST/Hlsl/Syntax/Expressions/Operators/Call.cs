using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators {
    // someFunction(arguments) 
    [Syntax] public partial record Call : Expression {
        private readonly Identifier                 _id;
        private readonly ArgumentList<Syntax<Hlsl>> _argList;
    }
}
