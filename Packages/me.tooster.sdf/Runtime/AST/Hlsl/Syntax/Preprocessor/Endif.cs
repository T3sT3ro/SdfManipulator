using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor {
    [SyntaxNode] public partial record Endif : PreprocessorSyntax {
        public EndIfKeyword endifKeyword { get; init; } = new();

        public override IReadOnlyList<SyntaxOrToken<hlsl>> ChildNodesAndTokens => new SyntaxOrToken<hlsl>[]
        {
            hashToken,
            endifKeyword,
            endOfDirectiveToken,
        };
    }
}
