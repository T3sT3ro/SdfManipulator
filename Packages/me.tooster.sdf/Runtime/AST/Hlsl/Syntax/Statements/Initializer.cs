using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    // = y
    // = {{a}, {b}}}
    [Syntax] public abstract partial record Initializer : Syntax<Hlsl> {
        [Init] private readonly EqualsToken _equalsToken;
        private readonly        Expression  _value;
    }
}
