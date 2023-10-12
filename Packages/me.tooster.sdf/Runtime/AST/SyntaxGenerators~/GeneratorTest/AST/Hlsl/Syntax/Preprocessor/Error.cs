using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor {
    [AstSyntax] public partial record Error : PreprocessorSyntax {
        public ErrorKeyword errorKeyword { get; init; } = new();
        public TokenString  tokenstring { get; init; } = new();
    }
}