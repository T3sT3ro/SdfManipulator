using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor {
    [Syntax] public partial record Endif : PreprocessorSyntax {
        [Init] private readonly EndIfKeyword _endifKeyword;
    }
}
