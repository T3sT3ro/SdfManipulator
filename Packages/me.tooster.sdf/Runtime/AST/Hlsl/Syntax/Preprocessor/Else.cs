using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor {
    [Syntax] public partial record Else : PreprocessorSyntax {
        [Init] private readonly ElseKeyword _elseKeyword;
    }
}
