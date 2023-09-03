#nullable enable
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor {
    [Syntax] public partial record Define : PreprocessorSyntax {
        [Init] private readonly DefineKeyword             _defineKeyword;
        [Init] private readonly ArgumentList<Identifier>? _argList;
        private readonly        Identifier                _id;
        [Init] private readonly TokenString               _tokenString;
    }
}
