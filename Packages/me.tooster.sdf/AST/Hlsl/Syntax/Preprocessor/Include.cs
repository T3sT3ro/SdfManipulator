using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Preprocessor {
    public partial record Include : PreprocessorSyntax {
        private readonly IncludePreprocessorKeyword _includeKeyword;
        private readonly QuotedStringLiteral        _filepath;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { hashToken, includeKeyword, filepath };
    }
}
