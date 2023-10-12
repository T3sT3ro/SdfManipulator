using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor {
    [AstSyntax] public partial record Endif : PreprocessorSyntax {
        public EndIfKeyword endifKeyword { get; init; } = new();
    }
}
