using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    [Syntax] public partial record Typedef : Statement {
        [Init] private readonly TypedefKeyword _typedefKeyword;
        private readonly        Type           _type;
        private readonly        Identifier     _id;
    }
}
