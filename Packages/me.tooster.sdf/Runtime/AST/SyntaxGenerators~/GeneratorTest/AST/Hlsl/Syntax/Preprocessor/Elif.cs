using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor {
    [AstSyntax] public partial record Elif : PreprocessorSyntax {
        public ElifKeyword elifKeyword { get; init; } = new();
        public TokenString condition { get; init; } = new();
    }
}
