#nullable enable
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor {
    [Syntax] public partial record Line : PreprocessorSyntax {
        [Init] private readonly LineKeyword          _lineKeyword;
        private readonly        IntLiteral           _lineNumber;
        private readonly        QuotedStringLiteral? _file;
    }
}
