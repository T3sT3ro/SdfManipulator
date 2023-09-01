using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor {
    public partial record Else : PreprocessorSyntax {
        private readonly ElseKeyword /*_*/elseKeyword;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { hashToken, elseKeyword };
    }
}
