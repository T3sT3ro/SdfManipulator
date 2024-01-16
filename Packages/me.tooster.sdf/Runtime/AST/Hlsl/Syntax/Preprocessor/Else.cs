using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor {
    [SyntaxNode] public partial record Else : PreprocessorSyntax {
        public ElseKeyword elseKeyword { get; init; } = new();

        public override IReadOnlyList<SyntaxOrToken<hlsl>> ChildNodesAndTokens => new SyntaxOrToken<hlsl>[]
        {
            hashToken,
            elseKeyword,
            endOfDirectiveToken,
        };
    }
}
