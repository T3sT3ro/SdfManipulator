using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Preprocessor {
    public partial record Else : PreprocessorSyntax {
        private readonly ElseKeyword _elseKeyword;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { hashToken, elseKeyword };
    }
}
