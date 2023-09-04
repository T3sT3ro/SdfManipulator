using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor {
    [Syntax] public partial record Ifndef : PreprocessorSyntax {
        [Init] private readonly IfndefKeyword _ifndefKeyword;
        private readonly        Identifier    _id;
    }
}
