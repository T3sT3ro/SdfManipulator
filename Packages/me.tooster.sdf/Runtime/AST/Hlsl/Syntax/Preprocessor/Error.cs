using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor {
    [SyntaxNode] public partial record Error : PreprocessorSyntax {
        public ErrorKeyword errorKeyword { get; init; } = new();
        public TokenString  tokenstring  { get; init; } = new();

        public override IReadOnlyList<SyntaxOrToken<hlsl>> ChildNodesAndTokens => new SyntaxOrToken<hlsl>[]
        {
            hashToken,
            errorKeyword,
            tokenstring,
            endOfDirectiveToken,
        };
    }
}
