using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor {
    [Syntax] public partial record If : PreprocessorSyntax {
        [Init] private readonly IfKeyword   _ifKeyword;
        private readonly        TokenString _condition;
    }
}
