using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor {
    public partial record Include : PreprocessorSyntax {
        private readonly IncludePreprocessorKeyword /*_*/includeKeyword;
        private readonly QuotedStringLiteral        /*_*/filepath;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { hashToken, includeKeyword, filepath };
    }
}
