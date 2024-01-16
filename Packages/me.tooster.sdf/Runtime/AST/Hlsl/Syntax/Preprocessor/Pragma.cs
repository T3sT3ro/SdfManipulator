#nullable enable
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor {
    [SyntaxNode] public partial record Pragma : PreprocessorSyntax {
        public PragmaKeyword pragmaKeyword { get; init; } = new();
        public TokenString?  tokenString   { get; init; } = new();
        
        public override IReadOnlyList<SyntaxOrToken<hlsl>> ChildNodesAndTokens => new SyntaxOrToken<hlsl>[]
        {
            hashToken,
            pragmaKeyword,
            tokenString,
            endOfDirectiveToken,
        }.Where(c => c is not null).Select(c => c!).ToList();
    }
}
