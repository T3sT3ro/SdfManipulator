using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor {
    [SyntaxNode] public partial record Include : PreprocessorSyntax {
        public IncludePreprocessorKeyword includeKeyword { get; init; } = new IncludeKeyword();
        public QuotedStringLiteral        filepath       { get; init; }

        public override IReadOnlyList<SyntaxOrToken<hlsl>> ChildNodesAndTokens() => new SyntaxOrToken<hlsl>[]
        {
            hashToken,
            includeKeyword,
            filepath,
            endOfDirectiveToken,
        };
    }
}
