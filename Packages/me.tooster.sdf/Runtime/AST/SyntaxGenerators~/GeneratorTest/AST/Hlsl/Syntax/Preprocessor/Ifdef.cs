using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor {
    [Syntax] public partial record Ifdef : PreprocessorSyntax {
        [Init] private readonly IfdefKeyword _ifdefKeyword;
        private readonly        Identifier   _id;
    }
}
