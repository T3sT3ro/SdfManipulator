using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor {
    [Syntax] public partial record Error : PreprocessorSyntax {
        [Init] private readonly ErrorKeyword _errorKeyword;
        [Init] private readonly TokenString  _tokenstring;
    }
}
