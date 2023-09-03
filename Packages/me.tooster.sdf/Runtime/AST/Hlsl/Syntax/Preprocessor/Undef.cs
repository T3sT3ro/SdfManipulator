using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor {
    [Syntax] public partial record Undef : PreprocessorSyntax {
        [Init] private readonly UndefKeyword _undefKeyword;
        private readonly        Identifier   _id;
    }
}
