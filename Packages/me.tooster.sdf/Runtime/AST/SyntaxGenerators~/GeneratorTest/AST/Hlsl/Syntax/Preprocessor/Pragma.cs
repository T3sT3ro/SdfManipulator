#nullable enable
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor {
    [Syntax] public partial record Pragma : PreprocessorSyntax {
        [Init] private readonly PragmaKeyword _pragmaKeyword;
        [Init] private readonly TokenString?  _tokenString;
    }
}
