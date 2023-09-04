using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor {
    [Syntax] public partial record Include : PreprocessorSyntax {
        [Init(With = typeof(IncludeKeyword))]
        private readonly IncludePreprocessorKeyword _includeKeyword;

        private readonly QuotedStringLiteral _filepath;
    }
}
