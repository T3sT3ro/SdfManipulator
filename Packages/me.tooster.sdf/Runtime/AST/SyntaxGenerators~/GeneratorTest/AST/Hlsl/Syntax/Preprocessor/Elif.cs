using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor {
    [Syntax] public partial record Elif : PreprocessorSyntax {
        [Init] private readonly ElifKeyword _elifKeyword;
        [Init] private readonly TokenString _condition;
    }
}
